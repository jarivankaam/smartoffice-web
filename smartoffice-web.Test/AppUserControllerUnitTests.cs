//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using Microsoft.Extensions.Logging;
//using Microsoft.AspNetCore.Mvc;
//using smartoffice_web.WebApi.Controllers;
//using smartoffice_web.WebApi.Repositories;
//using smartoffice_web.WebApi.Services;
//using smartoffice_web.WebApi.Models;
//using System;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Security.Claims;
//using Moq;
//using Moq.Language.Flow;
//using Moq.Language;
//using Moq.Protected;


//namespace smartoffice_web.Tests.Controllers
//{
//    [TestClass]
//    public class AppUserControllerTests
//    {
//        private Mock<IAppUserRepository> _mockAppUserRepository;
//        private Mock<IIdentityService> _mockIdentityService;
//        private Mock<ILogger<AppUserController>> _mockLogger;
//        private AppUserController _controller;

//        [TestInitialize]
//        public void Setup()
//        {
//            _mockAppUserRepository = new Mock<IAppUserRepository>();
//            _mockIdentityService = new Mock<IIdentityService>();
//            _mockLogger = new Mock<ILogger<AppUserController>>();
//            _controller = new AppUserController(_mockAppUserRepository.Object, _mockIdentityService.Object, _mockLogger.Object);
//        }

//        [TestMethod]
//        public async Task GetByIdentityUserId_UserExists_ReturnsOk()
//        {
//            // Arrange
//            var userId = "test-identity-user-id";
//            var user = new AppUser { Id = Guid.NewGuid(), IdentityUserId = userId };
//            _mockAppUserRepository.Setup(repo => repo.GetByIdentityUserIdAsync(userId)).ReturnsAsync(user);

//            // Act
//            var result = await _controller.GetByIdentityUserId(userId);

//            // Assert
//            var okResult = result.Result as OkObjectResult;
//            Assert.IsNotNull(okResult);
//            Assert.AreEqual(200, okResult.StatusCode);
//            Assert.AreEqual(user, okResult.Value);
//        }

//        [TestMethod]
//        public async Task GetByIdentityUserId_UserDoesNotExist_ReturnsNotFound()
//        {
//            // Arrange
//            var userId = "non-existent-user";
//            _mockAppUserRepository.Setup(repo => repo.GetByIdentityUserIdAsync(userId)).ReturnsAsync((AppUser)null);

//            // Act
//            var result = await _controller.GetByIdentityUserId(userId);

//            // Assert
//            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
//        }

//        [TestMethod]
//        public async Task GetCurrentUser_UserExists_ReturnsOk()
//        {
//            // Arrange
//            var identityUserId = "test-user-id";
//            var user = new AppUser { Id = Guid.NewGuid(), IdentityUserId = identityUserId };
//            _mockIdentityService.Setup(service => service.GetCurrentUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(identityUserId);
//            _mockAppUserRepository.Setup(repo => repo.GetByIdentityUserIdAsync(identityUserId)).ReturnsAsync(user);

//            // Act
//            var result = await _controller.GetCurrentUser();

//            // Assert
//            var okResult = result.Result as OkObjectResult;
//            Assert.IsNotNull(okResult);
//            Assert.AreEqual(200, okResult.StatusCode);
//            Assert.AreEqual(user.Id, okResult.Value);
//        }

//        [TestMethod]
//        public async Task GetCurrentUser_UserDoesNotExist_ReturnsNotFound()
//        {
//            // Arrange
//            var identityUserId = "unknown-user";
//            _mockIdentityService.Setup(service => service.GetCurrentUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(identityUserId);
//            _mockAppUserRepository.Setup(repo => repo.GetByIdentityUserIdAsync(identityUserId)).ReturnsAsync((AppUser)null);

//            // Act
//            var result = await _controller.GetCurrentUser();

//            // Assert
//            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
//        }

//        [TestMethod]
//        public async Task Create_ValidUser_ReturnsCreated()
//        {
//            // Arrange
//            var user = new AppUser { IdentityUserId = "new-user-id" };
//            _mockAppUserRepository.Setup(repo => repo.CreateAppUserAsync(It.IsAny<AppUser>())).ReturnsAsync(Guid.NewGuid());

//            // Act
//            var result = await _controller.Create(user);

//            // Assert
//            var createdResult = result.Result as CreatedAtActionResult;
//            Assert.IsNotNull(createdResult);
//            Assert.AreEqual(201, createdResult.StatusCode);
//        }

//        [TestMethod]
//        public async Task Create_NullUser_ReturnsBadRequest()
//        {
//            // Act
//            var result = await _controller.Create(null);

//            // Assert
//            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
//        }

//        [TestMethod]
//        public async Task GetUserWorlds_UserExists_ReturnsOk()
//        {
//            // Arrange
//            var userId = Guid.NewGuid();
//            var worlds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

//            _mockAppUserRepository
//     .Setup(repo => repo.GetUserWorlds(It.IsAny<Guid>()))
//     .ReturnsAsync(new List<Environment2D> { new Environment2D(), new Environment2D() }.AsEnumerable());


//            // Act
//            var result = await _controller.GetUserWorlds(userId);

//            // Assert
//            var okResult = result.Result as OkObjectResult;
//            Assert.IsNotNull(okResult);
//            Assert.AreEqual(200, okResult.StatusCode);
//            Assert.AreEqual(worlds, okResult.Value);
//        }


//        [TestMethod]
//        public async Task GetUserWorlds_UserDoesNotExist_ReturnsNotFound()
//        {
//            // Arrange
//            var userId = Guid.NewGuid();
//            _mockAppUserRepository
//                .Setup(repo => repo.GetUserWorlds(userId))
//                .ReturnsAsync(null as IEnumerable<Environment2D>); // Correct null handling

//            // Act
//            var result = await _controller.GetUserWorlds(userId);

//            // Assert
//            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
//        }

//    }
//}
