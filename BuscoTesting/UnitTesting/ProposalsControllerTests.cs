using BuscoAPI.Controllers;
using BuscoAPI.Entities;
using BuscoAPI.Services;
using BuscoTesting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuscoAPI.Tests.UnitTesting
{
    [TestClass]
    public  class ProposalsControllerTests : ConfigTesting
    {
        [TestMethod]
        public async Task GetProposal()
        {
                var nameDB = Guid.NewGuid().ToString();
                var context = BuildContext(nameDB);
                var mapper = ConfigAutoMapper();

                context.Proposals.Add(new Proposal()
                {
                    Id = 1,
                    Title = "Propuesta prueba",
                    Description = "Descripción prueba",
                    Requirements = "Requerimientos prueba",
                    Date = DateTime.Now,
                    MinBudget = 200,
                    MaxBudget = 400,
                    Ubication = new NetTopologySuite.Geometries.Point(new NetTopologySuite.Geometries.Coordinate(3.006897910592431, 39.57689753632767)),
                    userId = 1,
                    professionId = 1
                });
                await context.SaveChangesAsync();

                var context2 = BuildContext(nameDB);

                //Test
                var mockEnv = new Mock<IWebHostEnvironment>();
                var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
                var localFileStore = new LocalFileStore(mockEnv.Object, mockHttpContextAccessor.Object);

                var controller = new ProposalsController(context2, mapper, localFileStore);
                var response = await controller.Get(1);

                //Check
                var proposal = response.Value;
                //Assert.IsNotNull(proposal);
        }
    }
}
