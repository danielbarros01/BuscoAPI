﻿using BuscoAPI.Entities;

namespace BuscoAPI.Helpers
{
    public class HelperProfessions
    {
        //Para insertar y borrar profesiones
        public static async Task UpdateWorkerProfessions(int workerId, List<int> newProfessions, List<WorkersProfessions> workersProfessions, ApplicationDbContext context)
        {
            foreach (var professionId in newProfessions)
            {
                //Verifico si la profesion que le estoy pasando ya existe 
                //vinculada al usuario
                var hasProfession = workersProfessions.Exists(x => x.ProfessionId == professionId);
                if (!hasProfession)
                {
                    //Creamos la profesion
                    var workProfession = new WorkersProfessions
                    {
                        WorkerId = workerId,
                        ProfessionId = professionId
                    };

                    await context.WorkersProfessions.AddAsync(workProfession);
                }
            };

            //Borro profesiones ya no vinculadas
            foreach (var workProfession in workersProfessions)
            {
                //Existe la profesion en las nuevas profesiones?
                var existProfession = newProfessions.Any(x => x == workProfession.WorkerId);
                if (!existProfession)
                {
                    context.WorkersProfessions.Remove(workProfession);
                }
            }
        }
    }
}