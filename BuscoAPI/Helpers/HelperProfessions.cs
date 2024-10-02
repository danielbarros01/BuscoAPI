using BuscoAPI.Entities;

namespace BuscoAPI.Helpers
{
    public class HelperProfessions
    {
        //Para insertar y borrar profesiones
        public static async Task UpdateWorkerProfessions(int workerId, List<int> newProfessions, List<WorkersProfessions> workersProfessions, ApplicationDbContext context)
        {
            foreach (var professionId in newProfessions)
            {
                var hasProfession = workersProfessions.Exists(x => x.ProfessionId == professionId);
                if (!hasProfession)
                {
                    var workProfession = new WorkersProfessions
                    {
                        WorkerId = workerId,
                        ProfessionId = professionId
                    };

                    await context.WorkersProfessions.AddAsync(workProfession);
                }
            };

            foreach (var workProfession in workersProfessions)
            {
                var existProfession = newProfessions.Any(x => x == workProfession.ProfessionId);
                if (!existProfession)
                {
                    context.WorkersProfessions.Remove(workProfession);
                }
            }
        }
    }
}
