using AutoMapper;
using BuscoAPI.DTOS;
using BuscoAPI.DTOS.Worker;
using BuscoAPI.Entities;
using BuscoAPI.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
                var WorkProfession = new WorkersProfessions
                {
                    WorkerId = user.Id,
                    ProfessionId = workerCreation.ProfessionId
                };

                //Creo
                context.WorkersProfessions.Add(WorkProfession);
                context.Workers.Add(workerMapper);

                await context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex) {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }
    }
}
