﻿using AutoMapper;
using BuscoAPI.DTOS;
using BuscoAPI.DTOS.Proposals;
using BuscoAPI.DTOS.Users;
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
        private readonly string container = "proposals"; //folder name

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
                //Traigo al usuario
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) { return Unauthorized(); }

                //mapeo
                var proposal = mapper.Map<Proposal>(proposalCreation);
                proposal.userId = user.Id;

                //Guardar foto
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

                //Creo 
                context.Proposals.Add(proposal);

                await context.SaveChangesAsync();

                return Ok();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }

        }

        [HttpGet("{id}", Name = "GetProposal")]
        public async Task<ActionResult<Proposal>> Get(int id)
        {
            try
            {
                var proposal = await context.Proposals.FirstOrDefaultAsync(x => x.Id == id);

                if (proposal == null) { return NotFound(new ErrorInfo { Field = "Id", Message = "No existe tal propuesta" }); }

                return Ok(proposal);
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
                //Traigo al usuario
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) { return Unauthorized(); }

                var proposal = await context.Proposals.FirstOrDefaultAsync(x => x.Id == id && x.userId == user.Id);
                if (proposal == null) { return NotFound(new ErrorInfo { Field = "Error", Message = "No existe tal propuesta " }); }

                proposal = mapper.Map(proposalCreation, proposal);

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
        public async Task<ActionResult<List<Proposal>>> GetProposals([FromQuery] PaginationDTO pagination, int userid)
        {
            try
            {
                var queryable = context.Proposals.AsQueryable();
                await HttpContext.InsertPageParameters(queryable, pagination.NumberRecordsPerPage);

                var proposals = await queryable
                    .Where(x => x.userId == userid)
                    .Paginate(pagination)
                    .ToListAsync();

                return proposals;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }
    }
}