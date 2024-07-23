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
                //Traigo al usuario
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) { return Unauthorized(); }

                //Valido que sea trabajador
                var isWorker = await context.Workers.AnyAsync(w => w.UserId == user.Id);
                if (!isWorker)
                {
                    return Forbid();
                }

                //Traigo la propuesta y verifico que no sea nula y del mismo usuario
                var proposal = await context.Proposals
                    .FirstOrDefaultAsync(x => x.Id == proposalId);
                if (proposal == null) { return NotFound(new ErrorInfo { Field = "Error", Message = "No existe tal propuesta " }); }
                if (proposal.userId == user.Id) { return NotFound(new ErrorInfo { Field = "Error", Message = "No puedes postularte a una propuesta de tu auditoria" }); }


                //Verificar que no exista la aplicacion
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
                //Traigo al usuario
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) { return Unauthorized(); }

                //Traigo la propuesta
                var proposal = await context.Proposals
                    .FirstOrDefaultAsync(x => x.Id == proposalId);
                if (proposal == null) { return NotFound(new ErrorInfo { Field = "Error", Message = "No existe tal propuesta " }); }
                if (proposal.Status == false)
                {
                    //En proceso de trabajo
                    return StatusCode(403, new ErrorInfo { Message = "Ya hay un trabajador asignado" });
                }
                if (proposal.Status == true)
                {
                    //Ya terminada
                    return StatusCode(403, new ErrorInfo { Message = "Esta propuesta ya esta finalizada" });
                }

                //Verificar que exista la aplicacion
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

        //Dueno de la propuesta elige un postulante
        [HttpPatch("{proposalId}/{applicationId}", Name = "ChangeStatusApplication")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ChangeStatusApplication([FromQuery] bool status, int proposalId, int applicationId)
        {
            try
            {
                //Traigo al usuario
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) { return Unauthorized(); }

                //Traigo la propuesta
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
                    //En proceso de trabajo
                    return StatusCode(403, new ErrorInfo { Message = "Ya hay un trabajador asignado" });
                }
                if (proposal.Status == true)
                {
                    //Ya terminada
                    return StatusCode(403, new ErrorInfo { Message = "Esta propuesta ya esta finalizada" });
                }

                //Verificar que exista la aplicacion
                var application = await context.Applications
                    .FirstOrDefaultAsync(x => x.Id == applicationId && x.ProposalId == proposalId);

                if (application == null) { return NotFound(new ErrorInfo { Field = "Error", Message = "No existe la aplicacion" }); }

                //Actualizar estados
                if (status)
                {
                    application.Status = true; //Aplicacion aceptada
                    proposal.Status = false; // En proceso de trabajo
                }
                else if (!status)
                {
                    application.Status = false; //Aplicacion rechazada
                }

                // Guardar los cambios en la base de datos
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
        public async Task<ActionResult<List<Application>>> GetApplications([FromQuery] PaginationDTO pagination, int proposalId)
        {
            try
            {
                //Verificar que la propuesta existe
                var proposalExist = await context.Proposals.AnyAsync(x => x.Id == proposalId);
                if (!proposalExist)
                {
                    return NotFound(
                    new ErrorInfo { Field = "Error", Message = "No existe tal propuesta" });
                }


                var queryable = context.Applications
                    .Where(x => x.ProposalId == proposalId)
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
        public async Task<ActionResult<Application>> GetApplicationAccepted(int proposalId)
        {
            try
            {
                //Verificar que la propuesta existe
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

                var proposalDTO = mapper.Map<ApplicationDTO>(application);

                return application;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }
    }
}
