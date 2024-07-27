using AutoMapper;
using BuscoAPI.DTOS;
using BuscoAPI.DTOS.Proposals;
using BuscoAPI.Entities;
using BuscoAPI.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/*
 * Una propuesta para ser trabajo debe tener una aplicacion aceptada
 *finished: true, trabajos con propuestas terminadas
 *false: en proceso de trabajo
 */

namespace BuscoAPI.Controllers
{
    [ApiController]
    [Route("/api/jobs")]
    public class JobsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public JobsController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<Proposal>>> GetJobs(
            [FromQuery] PaginationDTO pagination,
            [FromQuery] bool finished)
        {
            try
            {
                //Traigo al usuario
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) { return Unauthorized(); }

                var queryable = context
                    .Proposals
                        .Where(p => p.userId == user.Id)
                        .Where(p => p.Status == finished)
                        .Include(p => p.Applications.Where(a => a.Status == true))
                            .ThenInclude(a => a.Worker)
                                .ThenInclude(w => w.User)
                        .Include(p => p.Applications.Where(a => a.Status == true))
                            .ThenInclude(a => a.Worker)
                                .ThenInclude(w => w.WorkersProfessions)
                                .ThenInclude(w => w.Profession)
                        .Where(p => p.Applications.Any(a => a.Status == true))
                        .AsQueryable();

                await HttpContext.InsertPageParameters(queryable, pagination.NumberRecordsPerPage);
                var proposals = await queryable.Paginate(pagination).ToListAsync();

                var mapperProposals = mapper.Map<List<ProposalDTO>>(proposals);

                return Ok(mapperProposals);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }


        [HttpGet("{userId}/completed")]
        public async Task<ActionResult<List<Proposal>>> GetJobsFinishedOfUser(
            int userId,
            [FromQuery] PaginationDTO pagination)
        {
            try
            {
                //la propuesta debe estar terminada, y en aplicaciones la que este seleccionada es el usuario
                var queryable = context.Proposals
                    .Where(p => p.Status == true && p.Applications.Any(a => a.Status == true && a.WorkerUserId == userId))
                    .AsQueryable();

                await HttpContext.InsertPageParameters(queryable, pagination.NumberRecordsPerPage);

                var proposals = await queryable.Paginate(pagination).ToListAsync();

                var mapperProposals = mapper.Map<List<ProposalDTO>>(proposals);

                return Ok(mapperProposals);
            }
            catch (Exception ex) {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }
    }
}
