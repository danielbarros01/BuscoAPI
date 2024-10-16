using BuscoAPI.Controllers;
using BuscoAPI.Entities;
using BuscoAPI.Services;
using BuscoTesting;
using Microsoft.Extensions.Configuration;
using Moq;


namespace BuscoAPI.Tests.UnitTesting
{
    [TestClass]
    public class UsersControllerTests : ConfigTesting
    {
        [TestMethod]
        public async Task CreateUser()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigAutoMapper();

            context.Users.Add(new User()
            {
                Id = 1,
                Username = "daniel10",
                Email = "dani@hotmail.com",
                Password = "password",
            });

            await context.SaveChangesAsync();

            var context2 = BuildContext(nameDB);

            var configuration = new Mock<IConfiguration>();
            var emailService = new Mock<IEmailService>();
            var fileStore = new Mock<IFileStore>();

            // Aquí configuramos los mocks si es necesario
            // Ejemplo: emailService.Setup(es => es.SomeMethod()).Returns(someValue);

            var controller = new UsersController(
                context2, configuration.Object, mapper,
                emailService.Object, fileStore.Object
            );

            var response = await controller.GetUser(1);
            var user = response.Value;

            Assert.IsNotNull(user);
            Assert.AreEqual(1, user.Id);
            Assert.AreEqual("daniel10", user.Username);
            Assert.AreEqual("dani@hotmail.com", user.Email);
            // Puedes agregar más aserciones según sea necesario
        }

    }
}
