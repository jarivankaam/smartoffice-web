using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using smartoffice_web.WebApi.Controllers;
using smartoffice_web.WebApi.Models;
using smartoffice_web.WebApi.Repositories;
using smartoffice_web.WebApi.Services;

namespace smartoffice_web.Tests.Controllers
{
    [TestClass]
    public class AppUserControllerTests
    {
        private Mock<IAppUserRepository> _mockAppUserRepository;
        private Mock<IIdentityService> _mockIdentityService;
        private Mock<ILogger<AppUserController>> _mockLogger;
        private AppUserController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockAppUserRepository = new Mock<IAppUserRepository>();
            _mockIdentityService = new Mock<IIdentityService>();
            _mockLogger = new Mock<ILogger<AppUserController>>();
            _controller = new AppUserController(_mockAppUserRepository.Object, _mockIdentityService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetByIdentityUserId_UserExists_ReturnsOk()
        {
            // ARRANGE
            var identityUserId = "test-identity-user-id";
            var user = new AppUser { Id = Guid.NewGuid(), IdentityUserId = identityUserId };
            _mockAppUserRepository
                .Setup(repo => repo.GetByIdentityUserIdAsync(identityUserId))
                .ReturnsAsync(user);

            // ACT
            var response = await _controller.GetByIdentityUserId(identityUserId);

            // ASSERT
            Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
            var okResult = response.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsInstanceOfType(okResult.Value, typeof(AppUser));
            var returnedUser = okResult.Value as AppUser;
            Assert.AreEqual(user.Id, returnedUser.Id);
        }

        [TestMethod]
        public async Task GetByIdentityUserId_UserDoesNotExist_ReturnsNotFound()
        {
            // ARRANGE
            var identityUserId = "non-existent-user";
            _mockAppUserRepository
                .Setup(repo => repo.GetByIdentityUserIdAsync(identityUserId))
                .ReturnsAsync((AppUser)null);

            // ACT
            var response = await _controller.GetByIdentityUserId(identityUserId);

            // ASSERT
            Assert.IsInstanceOfType(response.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetCurrentUser_UserExists_ReturnsOk()
        {
            // ARRANGE
            var identityUserId = "test-user-id";
            var user = new AppUser { Id = Guid.NewGuid(), IdentityUserId = identityUserId };
            _mockIdentityService
                .Setup(service => service.GetCurrentUserIdAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(identityUserId);
            _mockAppUserRepository
                .Setup(repo => repo.GetByIdentityUserIdAsync(identityUserId))
                .ReturnsAsync(user);

            // ACT
            var response = await _controller.GetCurrentUser();

            // ASSERT
            Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
            var okResult = response.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            // Assuming GetCurrentUser returns the user's Id
            Assert.AreEqual(user.Id, okResult.Value);
        }

        [TestMethod]
        public async Task GetCurrentUser_UserDoesNotExist_ReturnsNotFound()
        {
            // ARRANGE
            var identityUserId = "unknown-user";
            _mockIdentityService
                .Setup(service => service.GetCurrentUserIdAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(identityUserId);
            _mockAppUserRepository
                .Setup(repo => repo.GetByIdentityUserIdAsync(identityUserId))
                .ReturnsAsync((AppUser)null);

            // ACT
            var response = await _controller.GetCurrentUser();

            // ASSERT
            Assert.IsInstanceOfType(response.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Create_ValidUser_ReturnsCreated()
        {
            // ARRANGE
            var newUser = new AppUser { IdentityUserId = "new-user-id" };
            // Simulate a created user by returning a new Guid from the repository.
            _mockAppUserRepository
                .Setup(repo => repo.CreateAppUserAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(Guid.NewGuid());

            // ACT
            var response = await _controller.Create(newUser);

            // ASSERT
            Assert.IsInstanceOfType(response.Result, typeof(CreatedAtActionResult));
            var createdResult = response.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
        }

        [TestMethod]
        public async Task Create_NullUser_ReturnsBadRequest()
        {
            // ACT
            var response = await _controller.Create(null);

            // ASSERT
            Assert.IsInstanceOfType(response.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetUserWorlds_UserDoesNotExist_ReturnsNotFound()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            _mockAppUserRepository
                .Setup(repo => repo.GetUserWorlds(userId))
                .ReturnsAsync((IEnumerable<Environment2D>)null);

            // ACT
            var response = await _controller.GetUserWorlds(userId);

            // ASSERT
            Assert.IsInstanceOfType(response.Result, typeof(NotFoundResult));
        }
    }
}
