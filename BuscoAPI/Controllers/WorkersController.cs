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
    }
}


