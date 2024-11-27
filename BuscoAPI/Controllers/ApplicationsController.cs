using AutoMapper;
using BuscoAPI.DTOS;
using BuscoAPI.DTOS.Proposals;
using BuscoAPI.DTOS.Users;
using BuscoAPI.Entities;
using BuscoAPI.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq;

namespace BuscoAPI.Controllers
{
    [ApiController]
    [Route("api/applications")]
    public class ApplicationsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ApplicationsController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        [HttpPost("{proposalId}", Name = "AddApplication")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Create(int proposalId)
        {
            try
            {
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) { return Unauthorized(); }

                var isWorker = await context.Workers.AnyAsync(w => w.UserId == user.Id);
                if (!isWorker)
                {
                    return Forbid();
                }

                var proposal = await context.Proposals
                    .FirstOrDefaultAsync(x => x.Id == proposalId);
                if (proposal == null) { return NotFound(new ErrorInfo { Field = "Error", Message = "No existe tal propuesta " }); }
                if (proposal.userId == user.Id) { return NotFound(new ErrorInfo { Field = "Error", Message = "No puedes postularte a una propuesta de tu auditoria" }); }

                var applicationExist = await context.Applications
                    .AnyAsync(x => x.WorkerUserId == user.Id && x.ProposalId == proposal.Id);

                if (applicationExist) { return Conflict(new ErrorInfo { Message = "Ya ha aplicado a esta propuesta" }); }

                var application = new Application
                {
                    WorkerUserId = user.Id,
                    ProposalId = proposal.Id
                };

                context.Applications.Add(application);

                await context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpDelete("{proposalId}", Name = "RemoveApplication")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Remove(int proposalId)
        {
            try
            {
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) { return Unauthorized(); }

                var proposal = await context.Proposals
                    .FirstOrDefaultAsync(x => x.Id == proposalId);
                if (proposal == null) { return NotFound(new ErrorInfo { Field = "Error", Message = "No existe tal propuesta " }); }
                if (proposal.Status == false)
                {
                    return StatusCode(403, new ErrorInfo { Message = "Ya hay un trabajador asignado" });
                }
                if (proposal.Status == true)
                {
                    return StatusCode(403, new ErrorInfo { Message = "Esta propuesta ya esta finalizada" });
                }

                var application = await context.Applications
                    .FirstOrDefaultAsync(x => x.WorkerUserId == user.Id && x.ProposalId == proposalId);

                if (application == null) { return NotFound(new ErrorInfo { Field = "Error", Message = "No has aplicado a esta propuesta" }); }

                if (application.Status == true)
                {
                    return StatusCode(403, new ErrorInfo { Message = "El creador de la propuesta debe dar de baja tu aplicación antes de que puedas eliminarla" });
                }

                context.Remove(application);
                await context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPatch("{proposalId}/{applicationId}", Name = "ChangeStatusApplication")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ChangeStatusApplication([FromQuery] bool status, int proposalId, int applicationId)
        {
            try
            {
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) { return Unauthorized(); }

                var proposal = await context.Proposals
                    .FirstOrDefaultAsync(x => x.Id == proposalId);
                if (proposal == null)
                {
                    return NotFound(new ErrorInfo
                    {
                        Field = "Error",
                        Message = "No existe tal propuesta"
                    });
                }
                if (proposal.Status == false)
                {
                    return StatusCode(403, new ErrorInfo { Message = "Ya hay un trabajador asignado" });
                }
                if (proposal.Status == true)
                {
                    return StatusCode(403, new ErrorInfo { Message = "Esta propuesta ya esta finalizada" });
                }

                var application = await context.Applications
                    .FirstOrDefaultAsync(x => x.Id == applicationId && x.ProposalId == proposalId);

                if (application == null) { return NotFound(new ErrorInfo { Field = "Error", Message = "No existe la aplicacion" }); }

                if (status)
                {
                    application.Status = true; //Aplicacion aceptada
                    proposal.Status = false; // En proceso de trabajo
                }
                else if (!status)
                {
                    application.Status = false; //Aplicacion rechazada
                }

                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }


        [HttpGet("{proposalId}", Name = "GetApplications")]
        public async Task<ActionResult<List<Application>>> GetApplications(
            [FromQuery] PaginationDTO pagination, int proposalId)
        {
            try
            {
                var proposalExist = await context.Proposals.AnyAsync(x => x.Id == proposalId);
                if (!proposalExist)
                {
                    return NotFound(
                    new ErrorInfo { Field = "Error", Message = "No existe tal propuesta" });
                }


                var queryable = context.Applications
                    .Where(x => x.ProposalId == proposalId)
                    .Include(x => x.Worker)
                        .ThenInclude(w => w.Qualifications)
                    .Include(x => x.Worker)
                        .ThenInclude(w => w.User)
                    .AsQueryable();

                await HttpContext.InsertPageParameters(queryable, pagination.NumberRecordsPerPage);

                var applications = await queryable.Paginate(pagination).ToListAsync();

                var proposalDTO = mapper.Map<List<ApplicationDTO>>(applications);

                return Ok(proposalDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("{proposalId}/accepted", Name = "GetApplicationAccepted")]
        public async Task<ActionResult<ApplicationDTO>> GetApplicationAccepted(int proposalId)
        {
            try
            {
                var proposalExist = await context.Proposals.AnyAsync(x => x.Id == proposalId);
                if (!proposalExist)
                {
                    return NotFound(
                    new ErrorInfo { Field = "Error", Message = "No existe tal propuesta" });
                }

                var application = await context.Applications
                    .Include(x => x.Worker)
                    .ThenInclude(w => w.User)
                    .FirstOrDefaultAsync(x => x.Status == true && x.ProposalId == proposalId);

                var applicationDTO = mapper.Map<ApplicationDTO>(application);

                return applicationDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }



        [HttpGet("me", Name = "GetUserApplications")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<Application>>> GetApplicationsOfUser(
            [FromQuery] bool? status,
            [FromQuery] PaginationDTO pagination
            )
        {
            try
            {
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) { return Unauthorized(); }

                var queryable = context.Applications
                    .Where(x => x.WorkerUserId == user.Id)
                    .Where(x => (status == null && x.Status == null) || (status != null && x.Status == status))
                    .Include(x => x.Proposal)
                    .OrderByDescending(x => x.Date)
                    .AsQueryable();

                await HttpContext.InsertPageParameters(queryable, pagination.NumberRecordsPerPage);

                var applications = await queryable.Paginate(pagination).ToListAsync();

                var applicationsDTO = mapper.Map<List<ApplicationDTO>>(applications);

                return Ok(applicationsDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }
    }
}
