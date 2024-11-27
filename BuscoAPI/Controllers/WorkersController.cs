using AutoMapper;
using BuscoAPI.DTOS;
using BuscoAPI.DTOS.Proposals;
using BuscoAPI.DTOS.Users;
using BuscoAPI.DTOS.Worker;
using BuscoAPI.Entities;
using BuscoAPI.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Security.Claims;


/**
 * GetRecommendedWorkers: 
 *OrderByDescending y ThenByDescending se usan para ordenar los usuarios de manera que 
 *aquellos que coincidan con el usuario autenticado en City aparezcan primero, seguidos por 
 *los que coincidan en Department, luego en Province, y finalmente en Country.
 */


namespace BuscoAPI.Controllers
{
    [ApiController]
    [Route("/api/workers")]
    public class WorkersController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly GeometryFactory geometryFactory;

        public WorkersController(ApplicationDbContext context, IMapper mapper, GeometryFactory geometryFactory)
        {
            this.context = context;
            this.mapper = mapper;
            this.geometryFactory = geometryFactory;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Create([FromBody] WorkerCreationDTO workerCreation)
        {
            try
            {
                var userId = UtilItyAuth.GetUserIdFromClaims(HttpContext);
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (user == null) return Unauthorized();

                var workerExists = await context.Workers.AnyAsync(x => x.UserId == user.Id);
                if (workerExists) return Conflict(new ErrorInfo { Field = "Worker", Message = "El Trabajador ya esta registrado." });

                //Mapeo
                var workerMapper = mapper.Map<Worker>(workerCreation);
                workerMapper.UserId = user.Id;

                workerCreation.ProfessionsId.ForEach(professionId =>
                {
                    context.WorkersProfessions.Add(new WorkersProfessions
                    {
                        WorkerId = user.Id,
                        ProfessionId = professionId
                    });
                });

                context.Workers.Add(workerMapper);
                await context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Edit([FromBody] WorkerCreationDTO workerDto)
        {
            var worker = await GetEntity.GetWorker(HttpContext, context);
            if (worker == null) return NotFound(new ErrorInfo { Field = "Error", Message = "No esta registrado como trabajador" });

            var workersProfessions = await context.WorkersProfessions
                .Where(x => x.WorkerId == worker.UserId)
                .ToListAsync();


            var newProfessions = workerDto.ProfessionsId;
            await HelperProfessions.UpdateWorkerProfessions(worker.UserId, newProfessions, workersProfessions, context);

            worker = mapper.Map(workerDto, worker);
            context.Entry(worker).State = EntityState.Modified;

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkerDTO>> Get(int id)
        {
            try
            {
                var worker = await context.Workers.FirstOrDefaultAsync(x => x.UserId == id);
                if (worker == null) return NotFound();

                return mapper.Map<WorkerDTO>(worker);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("recommendations")]
        public async Task<ActionResult<List<WorkerDTO>>> GetRecommendedWorkers(
            [FromQuery] PaginationDTO pagination,
            [FromQuery] double? latitude,
            [FromQuery] double? longitude
            )
        {
            try
            {
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) return Unauthorized(new ErrorInfo { Field = "Error", Message = "Debe estar autenticado." });

                var ubicationSelected = (latitude.HasValue && longitude.HasValue)
                    ? geometryFactory.CreatePoint(new Coordinate(longitude.Value, latitude.Value))
                    : user.Ubication;

                var queryable = context.Workers
                    .Include(w => w.Qualifications)
                    .Include(w => w.WorkersProfessions)
                        .ThenInclude(wp => wp.Profession)
                    .Include(w => w.User)
                    .Where(w => w.UserId != user.Id)
                    .OrderBy(w => w.User.Ubication.Distance(ubicationSelected))
                    .AsNoTracking();

                await HttpContext.InsertPageParameters(queryable, pagination.NumberRecordsPerPage);

                var recommendedWorgingUsers = await queryable
                    .Paginate(pagination)
                    .ToListAsync();

                var mappedList = mapper.Map<List<WorkerDTO>>(recommendedWorgingUsers);

                return mappedList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }


        [HttpGet("search")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<WorkerWithQualification>>> SearchWorkers(
            [FromQuery] string query,
            [FromQuery] int? filterCategoryId,
            [FromQuery] int? filterQualification,
            [FromQuery] PaginationDTO pagination,
            [FromQuery] double? latitude,
            [FromQuery] double? longitude
            )
        {
            try
            {
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) return Unauthorized(new ErrorInfo { Field = "Error", Message = "Debe estar autenticado." });

                var queryable = context.Workers
                   .Where(w => w.UserId != user.Id)
                   .Include(w => w.User)
                   .Include(w => w.Qualifications)
                   .Include(w => w.WorkersProfessions)
                    .ThenInclude(wp => wp.Profession)
                   .AsNoTracking();


                if (filterCategoryId != null)
                {
                    queryable = queryable
                        .Where(w => w.WorkersProfessions
                        .Any(p => p.Profession.CategoryId == filterCategoryId));
                }

                if (!string.IsNullOrEmpty(query))
                {
                    queryable = queryable
                        .Where(w => w.WorkersProfessions
                            .Any(p => EF.Functions.Like(p.Profession.Name, $"%{query}%")));
                }

                if (filterQualification != null)
                {
                    queryable = queryable
                        .Where(w => w.Qualifications.Average(q => q.Score) >= filterQualification
                        && w.Qualifications.Average(q => q.Score) < filterQualification + 1);
                }

                var ubicationSelected = (latitude.HasValue && longitude.HasValue)
                    ? geometryFactory.CreatePoint(new Coordinate(longitude.Value, latitude.Value))
                    : user.Ubication;

                queryable = queryable.OrderBy(w => w.User.Ubication.Distance(ubicationSelected));

                await HttpContext.InsertNumberOfRecords(queryable);
                await HttpContext.InsertPageParameters(queryable, pagination.NumberRecordsPerPage);

                var workers = await queryable.Paginate(pagination).ToListAsync();

                var mapperWorkers = mapper.Map<List<WorkerDTO>>(workers);

                return Ok(mapperWorkers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, $"An error occurred {ex.Message}");
            }
        }

    }
}


