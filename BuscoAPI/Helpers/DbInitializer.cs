namespace BuscoAPI.Helpers
{
    public class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Asegurarse de que la base de datos está creada
            context.Database.EnsureCreated();

            // Sembrar categorías de profesiones
            if (!context.Categories.Any())
            {
                foreach (var category in DataSeed.Categories)
                {
                    context.Categories.Add(category);
                }
                context.SaveChanges();
            }

            // Sembrar profesiones
            if (!context.Professions.Any())
            {
                foreach (var profession in DataSeed.Professions)
                {
                    context.Professions.Add(profession);
                }
                context.SaveChanges();
            }
        }
    }
}
