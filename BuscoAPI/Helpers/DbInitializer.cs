using AutoMapper;
using Bogus;
using BuscoAPI.DTOS.Worker;
using BuscoAPI.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using static System.Net.Mime.MediaTypeNames;
using Application = BuscoAPI.Entities.Application;

namespace BuscoAPI.Helpers
{
    public class DbInitializer
    {
        public static void SeedCategoriesAndProfessions(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Categories.Any())
            {
                foreach (var category in DataSeed.Categories)
                {
                    context.Categories.Add(category);
                }
                context.SaveChanges();
            }

            if (!context.Professions.Any())
            {
                foreach (var profession in DataSeed.Professions)
                {
                    context.Professions.Add(profession);
                }
                context.SaveChanges();
            }
        }

        public static void SeedUsers(ApplicationDbContext context, GeometryFactory geometryFactory)
        {
            context.Database.EnsureCreated();

            var faker = new Faker<User>()
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Lastname, f => f.Name.LastName())
                .RuleFor(u => u.Username, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Password, f => "automatic")
                //fechas de nacimiento entre hace 50 años y hace 18 años.
                .RuleFor(u => u.Birthdate, f => f.Date.BetweenOffset(DateTime.Now.AddYears(-50), DateTime.Now.AddYears(-18)).DateTime)
                //.RuleFor(u => u.Image, f => f.Image.LoremFlickrUrl())
                .RuleFor(u => u.Image, f => $"https://i.pravatar.cc/150?u={f.Random.Guid()}")
                .RuleFor(u => u.Ubication, f => geometryFactory.CreatePoint(new Coordinate(f.Address.Longitude(), f.Address.Latitude())))
                .RuleFor(u => u.Confirmed, f => true);

            var users = faker.Generate(30);

            context.Users.AddRange(users);
            context.SaveChanges();
        }

        public static async Task SeedWorkers(ApplicationDbContext context, IMapper mapper)
        {
            // Asegurarse de que la base de datos está creada
            context.Database.EnsureCreated();
            int maxNumberUsers = 15;

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

        public static async Task SeedProposals(ApplicationDbContext context, GeometryFactory geometryFactory, int? userId = null)
        {
            context.Database.EnsureCreated();

            int maxNumberProposals = 15;

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
                .RuleFor(p => p.userId, _ => userId == null ? usersId[random.Next(usersId.Count)] : userId)
                .RuleFor(p => p.professionId, _ => random.Next(1, totalProfessions))
                .RuleFor(p => p.Ubication, f => geometryFactory.CreatePoint(new Coordinate(f.Address.Longitude(), f.Address.Latitude())));

            var proposals = faker.Generate(maxNumberProposals);

            context.Proposals.AddRange(proposals);
            context.SaveChanges();
        }

        //Crear aplicaciones para usuario en particular, para mostrar
        public static async Task SeedApplications(ApplicationDbContext context, int userId)
        {
            var faker = new Faker();

            context.Database.EnsureCreated();
            int maxNumberProposals = 15; //x propuestas
            int maxNumberApplicationsForProposal = 15; //x aplicaciones por cada propuesta

            //Traigo los trabajadores
            var workers = await context.Workers
                .Include(w => w.User)
                .Where(w => w.UserId != userId)
                .Take(maxNumberApplicationsForProposal)
                .ToListAsync();

            //Traigo las propuestas que sean status null
            var proposals = await context.Proposals
                .Where(p => p.Status == null && p.userId == userId)
                .Take(maxNumberProposals)
                .ToListAsync();

            List<Application> applications = [];

            foreach (var p in proposals)
            {
                foreach (var w in workers)
                {
                    var application = new Application
                    {
                        ProposalId = p.Id,
                        WorkerUserId = w.UserId,
                        Date = faker.Date.BetweenOffset(DateTime.Now.AddYears(-1), DateTime.Now.AddDays(-1)).DateTime,
                        Status = null
                    };

                    applications.Add(application);
                }
            }

            context.AddRange(applications);
            context.SaveChanges();
        }

        //Crear trabajos realizados y calificaciones
        public static async Task SeedQualifications(ApplicationDbContext context, GeometryFactory geometryFactory)
        {
            context.Database.EnsureCreated();
            int maxUsersNumber = 15;
            int numberOfWorkers = 10;
            int numberQualificationsForWorker = 5;
            int numberProposals = numberOfWorkers * numberQualificationsForWorker;

            //Creo propuestas
            #region Creo propuestas

            var users = await context.Users
                .Include(x => x.Worker)
                .Where(x => x.Worker == null && x.Password.Equals("automatic"))
                .Take(maxUsersNumber)
                .ToListAsync();

            var usersId = users.Select(u => u.Id).ToList();
            var random = new Random();
            var totalProfessions = await context.Professions.CountAsync();

            //Crear propuestas con status terminada
            var faker = new Faker<Proposal>()
                .RuleFor(p => p.Title, f => $"{f.Hacker.Adjective()} {f.Hacker.Noun()} {f.Hacker.Verb()}")
                .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
                .RuleFor(p => p.Requirements, f => f.Lorem.Paragraph())
                .RuleFor(p => p.Date, f => f.Date.BetweenOffset(DateTime.Now.AddYears(-1), DateTime.Now.AddDays(-1)).DateTime)
                .RuleFor(p => p.MinBudget, f => f.Finance.Amount(1000, 9000))
                .RuleFor(p => p.MaxBudget, f => f.Finance.Amount(10000, 1000000))
                .RuleFor(p => p.Image, f => f.Image.PicsumUrl())
                .RuleFor(p => p.Status, _ => true)
                .RuleFor(p => p.userId, _ => usersId[random.Next(usersId.Count)])
                .RuleFor(p => p.professionId, _ => random.Next(1, totalProfessions))
                .RuleFor(p => p.Ubication, f => geometryFactory.CreatePoint(new Coordinate(f.Address.Longitude(), f.Address.Latitude())));

            var proposals = faker.Generate(numberProposals);

            context.Proposals.AddRange(proposals);
            context.SaveChanges();
            #endregion

            //Traigo trabajadores
            var workers = await context.Workers
                .Include(w => w.User)
                .Where(w => w.User.Password.Equals("automatic"))
                .Take(numberOfWorkers)
                .ToListAsync();

            var applications = new List<Application>();
            var qualifications = new List<Qualification>();
            var fakerII = new Faker();

            for (int w=0, c = 0; c < numberProposals; c++,w++)
            {
                var worker = workers[w];
                var p = proposals[c];

                var application = new Application
                {
                    ProposalId = p.Id,
                    WorkerUserId = worker.UserId,
                    Date = fakerII.Date.BetweenOffset(DateTime.Now.AddYears(-1), DateTime.Now.AddDays(-1)).DateTime,
                    Status = true
                };

                var qualification = new Qualification
                {
                    Score = random.Next(1, 5),
                    Commentary = fakerII.Lorem.Lines(2),
                    Date = DateTime.Now,
                    UserId = p.userId,
                    WorkerUserId = worker.UserId
                };


                applications.Add(application);
                qualifications.Add(qualification);

                if ((w + 1) == numberOfWorkers && c < numberProposals) w = 0;
            }

            context.AddRange(applications);
            context.AddRange(qualifications);
            context.SaveChanges();
        }
    }
}
