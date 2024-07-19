using AutoMapper;
using BuscoAPI.DTOS;
using BuscoAPI.DTOS.Users;
using BuscoAPI.DTOS.Worker;
using BuscoAPI.Entities;
using BuscoAPI.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public WorkersController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Create([FromBody] WorkerCreationDTO workerCreation)
        {
            try
            {
                //Obtengo al usuario y valido que existe
                var userId = UtilItyAuth.GetUserIdFromClaims(HttpContext);
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (user == null) return Unauthorized();

                //Valido que no exista como trabajador
                var workerExists = await context.Workers.AnyAsync(x => x.UserId == user.Id);
                if (workerExists) return Conflict(new ErrorInfo { Field = "Worker", Message = "El Trabajador ya esta registrado." });
 
                //Mapeo
                var workerMapper = mapper.Map<Worker>(workerCreation);
                workerMapper.UserId = user.Id; //Le pongo el Id del usuario

                //Debo crear en la tabla Worker tiene cierta profesion
                workerCreation.ProfessionsId.ForEach(professionId =>
                {
                    //Creo las profesiones
                    context.WorkersProfessions.Add(new WorkersProfessions
                    {
                        WorkerId = user.Id,
                        ProfessionId = professionId
                    });
                });
                
                //Creo 
                context.Workers.Add(workerMapper);

                await context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex) {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Edit([FromBody] WorkerCreationDTO workerDto)
        {
            //traer usuario
            var worker = await GetEntity.GetWorker(HttpContext, context);
            if (worker == null) return NotFound(new ErrorInfo { Field = "Error", Message ="No esta registrado como trabajador"});

            var workersProfessions = await context.WorkersProfessions
                .Where(x => x.WorkerId == worker.UserId)
                .ToListAsync();


            var newProfessions = workerDto.ProfessionsId;
            //Agrego profesiones nuevas que no esten vinculadas
            //Borro profesiones ya no vinculadas
            await HelperProfessions.UpdateWorkerProfessions(worker.UserId, newProfessions, workersProfessions, context);

            // Mapeo las propiedades del DTO al objeto worker existente
            worker = mapper.Map(workerDto, worker);
            // Marco el estado del objeto worker como modificado en el contexto de la base de datos
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
        public async Task<ActionResult<List<UserDTO>>> GetRecommendedWorkers([FromQuery] PaginationDTO pagination)
        {
            try
            {
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) return Unauthorized(new ErrorInfo { Field = "Error", Message = "Debe estar autenticado." });

                var queryable = context.Users
                    .Include(x => x.Worker)
                    .ThenInclude(x => x.WorkersProfessions)
                    .ThenInclude(x => x.Profession)
                    .Where(x => x.Worker != null && x.Id != user.Id) //trabajador no sea null y no me incluya a mi
                    .OrderByDescending(x => x.City != null && x.City == user.City)
                    .ThenByDescending(x => x.Department != null && x.Department == user.Department)
                    .ThenByDescending(x => x.Province != null && x.Province == user.Province)
                    .ThenByDescending(x => x.Country != null && x.Country == user.Country)
                    .AsQueryable();

                await HttpContext.InsertPageParameters(queryable, pagination.NumberRecordsPerPage); 

                var recommendedWorgingUsers = await queryable.Paginate(pagination).ToListAsync();

                var mappedList = mapper.Map<List<UserDTO>>(recommendedWorgingUsers);

                return mappedList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }
    }
}


