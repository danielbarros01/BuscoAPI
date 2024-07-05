﻿using BuscoAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuscoAPI.Helpers
{
    public class GetEntity
    {
        public static async Task<User?> GetUser(HttpContext httpContext, ApplicationDbContext context)
        {
            //Obtengo al usuario y valido que existe
            var userId = UtilItyAuth.GetUserIdFromClaims(httpContext);

            // Validar que userId no sea nulo
            if (userId == null)
            {
                return null;
            }

            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            return user;
        }

        public static async Task<Worker?> GetWorker(HttpContext httpContext, ApplicationDbContext context)
        {
            //Obtengo al usuario y valido que existe
            var userId = UtilItyAuth.GetUserIdFromClaims(httpContext);

            // Validar que userId no sea nulo
            if (userId == null)
            {
                return null;
            }

            var worker = await context.Workers
                .Include(x => x.WorkersProfessions)
                .ThenInclude(x => x.Profession)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            return worker;
        }
    }
}