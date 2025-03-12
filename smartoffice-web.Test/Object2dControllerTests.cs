using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using smartoffice_web.WebApi.Controllers;
using smartoffice_web.WebApi.Models;
using smartoffice_web.WebApi.Repositories;

namespace smartoffice_web.Tests.Controllers
{
    [TestClass]
    public class Object2DControllerTests
    {
        private Mock<IObject2DRepository> _object2DRepositoryMock;
        private Mock<ILogger<Object2DController>> _loggerMock;
        private Object2DController _controller;

        [TestInitialize]
        public void Setup()
        {
            _object2DRepositoryMock = new Mock<IObject2DRepository>();
            _loggerMock = new Mock<ILogger<Object2DController>>();
            _controller = new Object2DController(_object2DRepositoryMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Get_ReturnsAllObjects()
        {
            // ARRANGE
            var objects = GenerateRandomObjects(3);
            _object2DRepositoryMock
                .Setup(repo => repo.GetAllObject2DsAsync())
                .ReturnsAsync(objects);

            // ACT
            var result = await _controller.Get();

            // ASSERT
            Assert.IsNotNull(result);
            Assert.AreEqual(3, ((List<Object2D>)result).Count);
        }

        [TestMethod]
        public async Task GetById_ObjectExists_ReturnsOkObjectResult()
        {
            // ARRANGE
            var obj = GenerateRandomObject("TestPrefab");
            _object2DRepositoryMock
                .Setup(repo => repo.GetObject2DByIdAsync(obj.Id))
                .ReturnsAsync(obj);

            // ACT
            var response = await _controller.GetById(obj.Id);

            // ASSERT
            Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
            var okResult = response.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(Object2D));
            var returnedObject = okResult.Value as Object2D;
            Assert.AreEqual(obj.Id, returnedObject.Id);
        }

        [TestMethod]
        public async Task GetById_ObjectDoesNotExist_ReturnsNotFoundResult()
        {
            // ARRANGE
            var nonExistingId = Guid.NewGuid();
            _object2DRepositoryMock
                .Setup(repo => repo.GetObject2DByIdAsync(nonExistingId))
                .ReturnsAsync((Object2D)null);

            // ACT
            var response = await _controller.GetById(nonExistingId);

            // ASSERT
            Assert.IsInstanceOfType(response.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Create_NullObject_ReturnsBadRequest()
        {
            // ACT
            var response = await _controller.Create(null);

            // ASSERT
            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Create_ValidObject_ReturnsCreatedAtActionResult()
        {
            // ARRANGE
            var obj = GenerateRandomObject("ValidPrefab");
            // Force a new Id assignment by setting the Id to empty.
            obj.Id = Guid.Empty;
            _object2DRepositoryMock
                .Setup(repo => repo.AddObject2DAsync(It.IsAny<Object2D>()))
                .Returns(Task.CompletedTask);

            // ACT
            var response = await _controller.Create(obj);

            // ASSERT
            Assert.IsInstanceOfType(response, typeof(CreatedAtActionResult));
            var createdResult = response as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.IsInstanceOfType(createdResult.Value, typeof(Object2D));
            var returnedObject = createdResult.Value as Object2D;
            Assert.AreNotEqual(Guid.Empty, returnedObject.Id);
        }

        [TestMethod]
        public async Task Update_NullObject_ReturnsBadRequest()
        {
            // ARRANGE
            var someId = Guid.NewGuid();

            // ACT
            var response = await _controller.Update(someId, null);

            // ASSERT
            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Update_IdMismatch_ReturnsBadRequest()
        {
            // ARRANGE
            var obj = GenerateRandomObject("MismatchPrefab");
            var differentId = Guid.NewGuid();

            // ACT
            var response = await _controller.Update(differentId, obj);

            // ASSERT
            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Update_ValidObject_ReturnsNoContent()
        {
            // ARRANGE
            var obj = GenerateRandomObject("UpdatePrefab");
            _object2DRepositoryMock
                .Setup(repo => repo.UpdateObject2DAsync(obj))
                .Returns(Task.CompletedTask);

            // ACT
            var response = await _controller.Update(obj.Id, obj);

            // ASSERT
            Assert.IsInstanceOfType(response, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNoContent()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            _object2DRepositoryMock
                .Setup(repo => repo.DeleteObject2DAsync(id))
                .Returns(Task.CompletedTask);

            // ACT
            var response = await _controller.Delete(id);

            // ASSERT
            Assert.IsInstanceOfType(response, typeof(NoContentResult));
        }

        // --------------------------
        // Private helper methods
        // --------------------------
        private List<Object2D> GenerateRandomObjects(int count)
        {
            var list = new List<Object2D>();
            for (int i = 0; i < count; i++)
            {
                list.Add(GenerateRandomObject($"Prefab_{i}"));
            }
            return list;
        }

        private Object2D GenerateRandomObject(string prefabId)
        {
            var random = new Random();
            return new Object2D
            {
                Id = Guid.NewGuid(),
                PrefabId = prefabId,
                PositionX = random.Next(0, 500),
                PositionY = random.Next(0, 500),
                ScaleX = random.Next(1, 10),
                ScaleY = random.Next(1, 10),
                RotationZ = random.Next(0, 360),
                SortingLayer = random.Next(0, 10),
                Environment2DID = random.Next(0, 2) == 0 ? (Guid?)null : Guid.NewGuid()
            };
        }
    }
}
