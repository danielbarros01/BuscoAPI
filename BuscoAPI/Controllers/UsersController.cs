using AutoMapper;
using BuscoAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscoAPI.Controllers
{
    [ApiController]
    [Route("/api/users")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;

        public UsersController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
        {
            this.context = context;
            this.configuration = configuration;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> Get()
        {
            var users = await context.Users.ToListAsync();
            return Ok(users);
        }
    }
}
