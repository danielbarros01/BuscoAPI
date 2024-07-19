using AutoMapper;
using Bogus;
using BuscoAPI.DTOS.Proposals;
using BuscoAPI.DTOS.Worker;
using BuscoAPI.Entities;
using BuscoAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace BuscoAPI.Helpers
{
    public class DbInitializer
    {
        public static void SeedCategoriesAndProfessions(ApplicationDbContext context)
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

        public static async Task SeedUsers(ApplicationDbContext context, SNDGService _sndgService)
        {
            // Asegurarse de que la base de datos está creada
            context.Database.EnsureCreated();

            //Traemos las provincias
            var provincias = await _sndgService.GetProvinces();
            var provinciasList = provincias.Provincias.Select(p => p.Nombre).ToList();

            var faker = new Faker<User>()
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Lastname, f => f.Name.LastName())
                .RuleFor(u => u.Username, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Password, f => "automatic")
                //fechas de nacimiento entre hace 50 años y hace 18 años.
                .RuleFor(u => u.Birthdate, f => f.Date.BetweenOffset(DateTime.Now.AddYears(-50), DateTime.Now.AddYears(-18)).DateTime)
                .RuleFor(u=> u.Image, f => f.Image.LoremFlickrUrl())
                .RuleFor(u => u.Country, f => "Argentina")
                .RuleFor(u => u.Province, f => f.PickRandom(provinciasList))
                .RuleFor(u => u.Department, (f, u) =>
                {
                    var departamentos = _sndgService.GetDepartments(u.Province).Result.Departamentos.Select(d => d.Nombre).ToList();
                    return f.PickRandom(departamentos);
                })
                .RuleFor(u => u.City, (f, u) =>
                {
                    var ciudades = _sndgService.GetCiudades(u.Province, u.Department).Result.localidades_censales.Select(c => c.Nombre).ToList();
                    return f.PickRandom(ciudades);
                })
                .RuleFor(u => u.Confirmed, f => true);

            var users = faker.Generate(10);

            context.Users.AddRange(users);
            context.SaveChanges();
        }


        public static async Task SeedWorkers(ApplicationDbContext context, IMapper mapper)
        {
            // Asegurarse de que la base de datos está creada
            context.Database.EnsureCreated();
            int maxNumberUsers = 5;

            // Traer de la base de datos un número de usuarios
            var users = await context.Users
                .Include(x => x.Worker)
                .Where(x => x.Worker == null && x.Password.Equals("automatic"))
                .Take(maxNumberUsers)
                .ToListAsync();

            var totalProfessions = await context.Professions.CountAsync();
            var random = new Random();
            var workers = new List<Worker>();

            foreach (var user in users)
            {
                int numberRandom = random.Next(1, totalProfessions);

                var faker = new Faker<WorkerCreationDTO>()
                    .RuleFor(w => w.Title, f => f.Name.JobTitle())
                    .RuleFor(w => w.YearsExperience, f => f.Random.Int(1, 20))
                    .RuleFor(w => w.Description, f => f.Lorem.Paragraph())
                    .RuleFor(w => w.ProfessionsId, _ => new List<int> { numberRandom });

                WorkerCreationDTO newWorker = faker.Generate();

                // Mapeo
                var workerMapper = mapper.Map<Worker>(newWorker);
                workerMapper.UserId = user.Id; // Asignar el Id del usuario
                workers.Add(workerMapper);

                // Crear las profesiones para el trabajador
                newWorker.ProfessionsId.ForEach(professionId =>
                {
                    context.WorkersProfessions.Add(new WorkersProfessions
                    {
                        WorkerId = workerMapper.UserId,
                        ProfessionId = professionId
                    });
                });
            }

            context.Workers.AddRange(workers);
            await context.SaveChangesAsync();
        }

        public static async Task SeedProposals(ApplicationDbContext context)
        {
            // Asegurarse de que la base de datos está creada
            context.Database.EnsureCreated();

            int maxNumberProposals = 15;

            // Traer de la base de datos un número de usuarios
            var users = await context.Users
                .Include(x => x.Worker)
                .Where(x => x.Worker == null && x.Password.Equals("automatic"))
                .Take(maxNumberProposals)
                .ToListAsync();
            var usersId = users.Select(u => u.Id).ToList();

            var totalProfessions = await context.Professions.CountAsync();

            var random = new Random();

            var faker = new Faker<Proposal>()
                .RuleFor(p => p.Title, f => $"{f.Hacker.Adjective()} {f.Hacker.Noun()} {f.Hacker.Verb()}")
                .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
                .RuleFor(p => p.Requirements, f => f.Lorem.Paragraph())
                .RuleFor(p => p.Date, f => f.Date.BetweenOffset(DateTime.Now.AddYears(-1), DateTime.Now.AddDays(-1)).DateTime)
                .RuleFor(p => p.MinBudget, f => f.Finance.Amount(1000, 9000))
                .RuleFor(p => p.MaxBudget, f => f.Finance.Amount(10000, 1000000))
                .RuleFor(p => p.Image, f => f.Image.PicsumUrl())
                .RuleFor(p => p.Status, _ => null)
                .RuleFor(p => p.userId, _ => usersId[random.Next(usersId.Count)]) //Un id al azar
                .RuleFor(p => p.professionId, _ => random.Next(1, totalProfessions)); //Un id al azar

            var proposals = faker.Generate(10);

            context.Proposals.AddRange(proposals);
            context.SaveChanges();
        }
    }
}
