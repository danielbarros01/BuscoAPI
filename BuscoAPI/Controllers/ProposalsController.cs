using AutoMapper;
using BuscoAPI.DTOS;
using BuscoAPI.DTOS.Proposals;
using BuscoAPI.DTOS.Worker;
using BuscoAPI.Entities;
using BuscoAPI.Helpers;
using BuscoAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscoAPI.Controllers
{
    [ApiController]
    [Route("api/proposals")]
    public class ProposalsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStore fileStore;
        private readonly string container = "proposals"; 

        public ProposalsController(ApplicationDbContext context, IMapper mapper, IFileStore fileStore)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStore = fileStore;
        }

        [HttpPost(Name = "CreateProposal")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Create([FromForm] ProposalCreationDTO proposalCreation)
        {
            try
            {
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) { return Unauthorized(); }

                var proposal = mapper.Map<Proposal>(proposalCreation);
                proposal.userId = user.Id;

                var image = proposalCreation.Image;
                if (image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        var content = memoryStream.ToArray(); //datos en bytes
                        var extension = Path.GetExtension(image.FileName);

                        proposal.Image = await fileStore.SaveFile(
                                content,
                                extension,
                                container,
                                image.ContentType
                             );
                    }
                }

                context.Proposals.Add(proposal);
                await context.SaveChangesAsync();

                return new CreatedAtRouteResult(
                    "GetProposal",
                    new { id = proposal.Id }
                );

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }

        }

        [HttpGet("{id}", Name = "GetProposal")]
        public async Task<ActionResult<ProposalDTO>> Get(int id)
        {
            try
            {
                var proposal = await context.Proposals
                    .Include(x => x.profession)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (proposal == null) { return NotFound(new ErrorInfo { Field = "Id", Message = "No existe tal propuesta" }); }

                var dto = mapper.Map<ProposalDTO>(proposal);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPut("{id}", Name = "EditProposal")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(int id, [FromForm] ProposalCreationDTO proposalCreation)
        {
            try
            {
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) { return Unauthorized(); }

                var proposal = await context.Proposals.FirstOrDefaultAsync(x => x.Id == id && x.userId == user.Id);
                if (proposal == null) { return NotFound(new ErrorInfo { Field = "Error", Message = "No existe tal propuesta " }); }

                proposalCreation.Status = proposal.Status; //ProposalCreationStatus es null por defecto, por eso hacemos esto

                proposal = mapper.Map(proposalCreation, proposal);

                var image = proposalCreation.Image;
                if (image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        var content = memoryStream.ToArray(); 
                        var extension = Path.GetExtension(image.FileName);

                        proposal.Image = await fileStore.SaveFile(
                                content,
                                extension,
                                container,
                                image.ContentType
                             );
                    }
                }

                context.Entry(proposal).State = EntityState.Modified;

                await context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpDelete("{id}", Name = "DeleteProposal")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) { return Unauthorized(); }

                var proposal = await context.Proposals.FirstOrDefaultAsync(x => x.Id == id && x.userId == user.Id);
                if (proposal == null) { return NotFound(new ErrorInfo { Field = "Error", Message = "No existe tal propuesta " }); }

                context.Remove(proposal);
                await context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("all/{userid}", Name = "GetProposalsOfUser")]
        public async Task<ActionResult<List<ProposalDTO>>> GetProposals(
            [FromQuery] PaginationDTO pagination,
            [FromQuery] bool? status,
            int userid)
        {
            try
            {
                var queryable = context.Proposals
                    .Where(x => x.userId == userid)
                    .OrderByDescending(x => x.Date)
                    .AsQueryable();

                await HttpContext.InsertPageParameters(queryable, pagination.NumberRecordsPerPage);

                List<Proposal> proposals;

                if (status == null)
                {
                    //La propuesta esta activa
                    proposals = await queryable
                        .Where(x => x.Status == null)
                        .Paginate(pagination)
                        .ToListAsync();
                }
                else if (status == true)
                {
                    //La propuesta esta terminada
                    proposals = await queryable
                       .Where(x => x.Status == true)
                       .Paginate(pagination)
                       .ToListAsync();
                }
                else
                {
                    //La propuesta esta en proceso de realizacion
                    proposals = await queryable
                       .Where(x => x.Status == false)
                       .Paginate(pagination)
                       .ToListAsync();
                }

                return mapper.Map<List<ProposalDTO>>(proposals);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("recommendations")]
        public async Task<ActionResult<List<ProposalDTO>>> GetRecommendedProposals(
            [FromQuery] PaginationDTO pagination
            )
        {
            try
            {
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) return Unauthorized(new ErrorInfo { Field = "Error", Message = "Debe estar autenticado." });

                var isWorker = await context.Workers.AnyAsync(w => w.UserId == user.Id);
                if (!isWorker)
                {
                    return Forbid();
                }

                var queryable = context.Proposals
                    .Where(x => x.Status == null && x.userId != user.Id) //null significa que esta sin trabajador, ergo Disponible
                    .Include(x => x.user)
                    .Include(x => x.profession)
                    .OrderBy(x => x.Ubication.Distance(user.Ubication))
                    .AsQueryable();

                await HttpContext.InsertPageParameters(queryable, pagination.NumberRecordsPerPage);

                var recommendedProposals = await queryable.Paginate(pagination).ToListAsync();

                return mapper.Map<List<ProposalDTO>>(recommendedProposals);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }


        [HttpPatch("{id}/finalize")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> FinalizeProposal(int id)
        {
            try
            {
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) return Unauthorized(new ErrorInfo { Field = "Error", Message = "Debe estar autenticado." });

                var proposal = await context.Proposals
                    .Include(p => p.Applications)
                    .FirstOrDefaultAsync(p => p.Id == id && p.userId == user.Id);

                if (proposal == null)
                {
                    return NotFound(new ErrorInfo { Field = "Error", Message = "No existe tal propuesta" });
                }

                //False = 0, esta en proceso
                if (proposal.Status != false)
                {
                    return StatusCode(403, new ErrorInfo { Message = "La propuesta debe tener un trabajador asignado" });
                }

                proposal.Status = true; // propuesta finalizada
                await context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred");
            }
        }


        [HttpPatch("{proposalId}/remove-application", Name = "RemoveApplicationForProposal")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> RemoveApplicationForProposal(int proposalId)
        {
            try
            {
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) { return Unauthorized(); }

                var proposal = await context.Proposals.FirstOrDefaultAsync(x => x.Id == proposalId && x.userId == user.Id);
                if (proposal == null) { return NotFound(new ErrorInfo { Field = "Error", Message = "No existe tal propuesta" }); }

                var application = await context.Applications
                    .FirstOrDefaultAsync(x => x.ProposalId == proposalId && x.Status == true);

                //Actualizo estados
                proposal.Status = null; //Sin trabajador
                application.Status = false; //rechazado

                await context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }


        [HttpGet("search")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<WorkerWithQualification>>> SearchProposals(
            [FromQuery] string query,
            [FromQuery] int? filterCategoryId,
            [FromQuery] Ubication ubication,
            [FromQuery] PaginationDTO pagination
            )
        {
            try
            {
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) return Unauthorized(new ErrorInfo { Field = "Error", Message = "Debe estar autenticado." });


                var queryable = context.Proposals
                   .Where(p => p.userId != user.Id)
                   .Include(p => p.profession)
                   .AsNoTracking();


                if (filterCategoryId != null)
                {
                    queryable = queryable.Where(p => p.profession.CategoryId == filterCategoryId);
                }

                if (!string.IsNullOrEmpty(query))
                {
                    queryable = queryable.Where(x => EF.Functions.Like(x.profession.Name, $"%{query}%"));
                }
                queryable = queryable
                    .OrderBy(x => x.Ubication.Distance(user.Ubication))
                    .ThenByDescending(p => p.Date);

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

    }
}
