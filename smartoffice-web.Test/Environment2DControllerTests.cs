using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using smartoffice_web.WebApi.Controllers;
using smartoffice_web.WebApi.Models;
using smartoffice_web.WebApi.Repositories;

namespace smartoffice_web.Tests
{
    [TestClass]
    public sealed class Environment2DControllerTests
    {
        // Private mocks that will be set up per test
        private Mock<IEnvironment2DRepository> _environment2DRepository;
        private Mock<Microsoft.Extensions.Logging.ILogger<Environment2DController>> _logger;
        private Mock<IActionDescriptorCollectionProvider> _actionDescriptorProvider;

        [TestInitialize]
        public void Setup()
        {
            _environment2DRepository = new Mock<IEnvironment2DRepository>();
            _logger = new Mock<Microsoft.Extensions.Logging.ILogger<Environment2DController>>();
            _actionDescriptorProvider = new Mock<IActionDescriptorCollectionProvider>();
        }

        [TestMethod]
        public async Task GetById_ExistingEnvironment_ReturnsOkObjectResult()
        {
            // ARRANGE
            var environment = GenerateRandomEnvironment("Test Environment");
            _environment2DRepository.Setup(x => x.GetWorldByIdAsync(environment.Id))
                .ReturnsAsync(environment);

            var controller = new Environment2DController(
                _environment2DRepository.Object,
                _logger.Object,
                _actionDescriptorProvider.Object);

            // ACT
            var response = await controller.GetById(environment.Id);

            // ASSERT
            Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
            var okResult = response.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(Environment2D));
            var returnedEnvironment = okResult.Value as Environment2D;
            Assert.AreEqual(environment.Id, returnedEnvironment.Id);
        }

        [TestMethod]
        public async Task GetById_NonExistingEnvironment_ReturnsNotFoundResult()
        {
            // ARRANGE
            var nonExistingId = Guid.NewGuid();
            _environment2DRepository.Setup(x => x.GetWorldByIdAsync(nonExistingId))
                .ReturnsAsync((Environment2D)null);

            var controller = new Environment2DController(
                _environment2DRepository.Object,
                _logger.Object,
                _actionDescriptorProvider.Object);

            // ACT
            var response = await controller.GetById(nonExistingId);

            // ASSERT
            Assert.IsInstanceOfType(response.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Create_InvalidAppUserId_ReturnsBadRequest()
        {
            // ARRANGE
            var environment = GenerateRandomEnvironment("Invalid Environment");
            // Set invalid AppUserId (empty)
            environment.AppUserId = Guid.Empty;

            var controller = new Environment2DController(
                _environment2DRepository.Object,
                _logger.Object,
                _actionDescriptorProvider.Object);

            // ACT
            var response = await controller.Create(environment);

            // ASSERT
            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
            var badRequestResult = response as BadRequestObjectResult;
            Assert.AreEqual("Invalid or missing AppUserId.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task Create_ValidEnvironment_ReturnsCreatedAtActionResult()
        {
            // ARRANGE
            var environment = GenerateRandomEnvironment("Valid Environment");
            // Ensure the Id is empty so that the controller assigns a new one
            environment.Id = Guid.Empty;

            // Setup repository to complete without exception.
            _environment2DRepository.Setup(x => x.AddWorldAsync(It.IsAny<Environment2D>()))
                .Returns(Task.CompletedTask);

            var controller = new Environment2DController(
                _environment2DRepository.Object,
                _logger.Object,
                _actionDescriptorProvider.Object);

            // ACT
            var response = await controller.Create(environment);

            // ASSERT
            Assert.IsInstanceOfType(response, typeof(CreatedAtActionResult));
            var createdResult = response as CreatedAtActionResult;
            Assert.IsNotNull(createdResult.Value);
            Assert.IsInstanceOfType(createdResult.Value, typeof(Environment2D));
            var returnedEnvironment = createdResult.Value as Environment2D;
            // The controller should have assigned a new Id.
            Assert.AreNotEqual(Guid.Empty, returnedEnvironment.Id);
            // The assigned Id should be the one in the returned environment.
            Assert.AreEqual(returnedEnvironment.Id, environment.Id);
        }

        [TestMethod]
        public async Task Create_ExceptionDuringAdd_ReturnsInternalServerError()
        {
            // ARRANGE
            var environment = GenerateRandomEnvironment("Exception Environment");
            _environment2DRepository.Setup(x => x.AddWorldAsync(It.IsAny<Environment2D>()))
                .ThrowsAsync(new Exception("Database error"));

            var controller = new Environment2DController(
                _environment2DRepository.Object,
                _logger.Object,
                _actionDescriptorProvider.Object);

            // ACT
            var response = await controller.Create(environment);

            // ASSERT
            Assert.IsInstanceOfType(response, typeof(ObjectResult));
            var objectResult = response as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [TestMethod]
        public async Task Update_NullEnvironment_ReturnsBadRequest()
        {
            // ARRANGE
            var controller = new Environment2DController(
                _environment2DRepository.Object,
                _logger.Object,
                _actionDescriptorProvider.Object);
            var someId = Guid.NewGuid();

            // ACT
            var response = await controller.Update(someId, null);

            // ASSERT
            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Update_IdMismatch_ReturnsBadRequest()
        {
            // ARRANGE
            var environment = GenerateRandomEnvironment("Mismatch Environment");
            var differentId = Guid.NewGuid();
            var controller = new Environment2DController(
                _environment2DRepository.Object,
                _logger.Object,
                _actionDescriptorProvider.Object);

            // ACT
            var response = await controller.Update(differentId, environment);

            // ASSERT
            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Update_ValidEnvironment_ReturnsNoContent()
        {
            // ARRANGE
            var environment = GenerateRandomEnvironment("Update Environment");
            _environment2DRepository.Setup(x => x.UpdateWorldAsync(environment))
                .Returns(Task.CompletedTask);

            var controller = new Environment2DController(
                _environment2DRepository.Object,
                _logger.Object,
                _actionDescriptorProvider.Object);

            // ACT
            var response = await controller.Update(environment.Id, environment);

            // ASSERT
            Assert.IsInstanceOfType(response, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNoContent()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            _environment2DRepository.Setup(x => x.DeleteWorldAsync(id))
                .Returns(Task.CompletedTask);

            var controller = new Environment2DController(
                _environment2DRepository.Object,
                _logger.Object,
                _actionDescriptorProvider.Object);

            // ACT
            var response = await controller.Delete(id);

            // ASSERT
            Assert.IsInstanceOfType(response, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Get_ReturnsAllEnvironments()
        {
            // ARRANGE
            var environments = GenerateRandomEnvironments(3);
            _environment2DRepository.Setup(x => x.GetAllEnvironment2DsAsync())
                .ReturnsAsync(environments);

            var controller = new Environment2DController(
                _environment2DRepository.Object,
                _logger.Object,
                _actionDescriptorProvider.Object);

            // ACT
            var result = await controller.Get();

            // ASSERT
            // Since Get returns IEnumerable<Environment2D> directly, we verify the count.
            Assert.AreEqual(3, ((List<Environment2D>)result).Count);
        }

        [TestMethod]
        public async Task GetObjects_ExistingWorld_ReturnsOkObjectResult()
        {
            // ARRANGE
            var worldId = Guid.NewGuid();
            var objects = new List<Object2D> { new Object2D { Id = Guid.NewGuid() } };
            _environment2DRepository.Setup(x => x.GetObjectsForWorld(worldId))
                .ReturnsAsync(objects);

            var controller = new Environment2DController(
                _environment2DRepository.Object,
                _logger.Object,
                _actionDescriptorProvider.Object);

            // ACT
            var response = await controller.GetObjects(worldId);

            // ASSERT
            Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
            var okResult = response.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(objects, okResult.Value);
        }

        [TestMethod]
        public async Task GetObjects_NonExistingWorld_ReturnsNotFoundResult()
        {
            // ARRANGE
            var worldId = Guid.NewGuid();
            _environment2DRepository.Setup(x => x.GetObjectsForWorld(worldId))
                .ReturnsAsync((IEnumerable<Object2D>)null);

            var controller = new Environment2DController(
                _environment2DRepository.Object,
                _logger.Object,
                _actionDescriptorProvider.Object);

            // ACT
            var response = await controller.GetObjects(worldId);

            // ASSERT
            Assert.IsInstanceOfType(response.Result, typeof(NotFoundResult));
        }

        // --------------------------
        // Private helper methods
        // --------------------------
        private List<Environment2D> GenerateRandomEnvironments(int count)
        {
            var list = new List<Environment2D>();
            for (int i = 0; i < count; i++)
            {
                list.Add(GenerateRandomEnvironment($"Random Environment {i}"));
            }
            return list;
        }

        private Environment2D GenerateRandomEnvironment(string name)
        {
            var random = new Random();
            return new Environment2D
            {
                Id = Guid.NewGuid(),
                AppUserId = Guid.NewGuid(),
                MaxHeight = random.Next(1, 100),
                MaxWidth = random.Next(1, 100),
                Name = name
            };
        }
    }
}
