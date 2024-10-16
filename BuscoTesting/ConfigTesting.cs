using AutoMapper;
using BuscoAPI;
using BuscoAPI.Helpers;
using BuscoAPI.Services;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;

namespace BuscoTesting
{
    public class ConfigTesting
    {
        protected ApplicationDbContext BuildContext(string nameDB)
        {

            var context = new ApplicationDbContext
                (
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(nameDB).Options
                );

            return context;
        }

        protected IMapper ConfigAutoMapper()
        {
            var config =
                new MapperConfiguration(options =>
                {
                    var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                    options.AddProfile(new AutoMapperProfiles(geometryFactory));
                });

            return config.CreateMapper();
        }
    }
}