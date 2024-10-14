using AutoMapper;
using BuscoAPI.DTOS;
using BuscoAPI.Entities;
using BuscoAPI.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BuscoAPI.Controllers
{
    [ApiController]
    [Route("api/qualifications")]
    public class QualificationsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public QualificationsController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateQualification([FromBody] QualificationCreationDTO qualificationCreation)
        {
            try
            {
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) { return Unauthorized(); }

                var qualificationExists = await context.WorkerQualification
                    .AnyAsync(q => q.UserId == qualificationCreation.WorkerUserId && q.UserId == user.Id);

                if (qualificationExists)
                {
                    return Conflict(new ErrorInfo { Message = "Ya has calificado a este trabajador" });
                }

                var qualification = mapper.Map<Qualification>(qualificationCreation);
                qualification.UserId = user.Id;

                context.WorkerQualification.Add(qualification);

                await context.SaveChangesAsync();

                return Created();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("{workerId}")]
        public async Task<ActionResult> GetQualifications(
            int workerId,
            [FromQuery] PaginationDTO pagination,
            [FromQuery] int? stars
            )
        {
            try
            {
                var totalQueryable = context.WorkerQualification
                    .Where(q => q.WorkerUserId == workerId)
                    .AsQueryable();

                var count = await totalQueryable.CountAsync();

                double averageScore = 0;

                Dictionary<int, int> ratingFrequencies = new Dictionary<int, int>();

                if (count > 0)
                {
                    averageScore = await totalQueryable.AverageAsync(q => q.Score);

                    ratingFrequencies = await totalQueryable
                        .GroupBy(q => q.Score)
                        .Select(g => new { Score = (int)g.Key, Frequency = g.Count() })
                        .ToDictionaryAsync(g => g.Score, g => g.Frequency);

                    for (int i = 1; i <= 5; i++)
                    {
                        if (!ratingFrequencies.ContainsKey(i))
                        {
                            ratingFrequencies[i] = 0;
                        }
                    }
                }

                var queryable = totalQueryable
                    .Where(q => stars != null ? q.Score == (float)stars : q.Score > 0)
                    .Include(q => q.User) 
                    .OrderByDescending(q => q.Date)
                    .AsQueryable();

                await HttpContext.InsertPageParameters(queryable, pagination.NumberRecordsPerPage);

                var qualifications = await queryable
                    .Paginate(pagination)
                    .ToListAsync();

                var mappedQualifications = mapper.Map<List<QualificationDTO>>(qualifications);

                return Ok(new
                {
                    Quantity = count,
                    Average = averageScore,
                    RatingFrequencies = ratingFrequencies,
                    Qualifications = mappedQualifications
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }
    }
}
