﻿using BuscoAPI.DTOS;
using BuscoAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscoAPI.Controllers
{
    [ApiController]
    [Route("/api/professions")]
    public class ProfessionsController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public ProfessionsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("{id}", Name = "ObtenerProfesion")]
        public async Task<ActionResult<Profession>> Get(int id)
        {
            var profession = await context.Professions
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (profession == null) {
                return NotFound(new ErrorInfo { Field = "Id", Message = "La profesión solicitada no existe"});
            }

            return Ok(profession);
        }

        [HttpGet("categories", Name = "ObtenerCategorias")]
        public async Task<ActionResult<List<ProfessionCategory>>> GetCategories()
        {
            var categories = await context.Categories.ToListAsync();

            return Ok(categories);
        }

        [HttpGet("categories/{categoryId}", Name ="ObtenerProfesiones")]
        public async Task<ActionResult<ProfessionCategory>> GetProfessionsForCategory(int categoryId)
        {
            var categoryExists = await context.Categories.AnyAsync(x => x.Id == categoryId);

            if (!categoryExists) { 
                return BadRequest(new ErrorInfo { Field = "categoryId", Message = $"No existe dicha categoría" });
            }

            var professions = await context.Categories
                .Include(x => x.Proffesions)
                .FirstOrDefaultAsync(x => x.Id == categoryId);

            return Ok(professions);
        }
    }
}
