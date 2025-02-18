using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using smartoffice_web.WebApi.Models;
using smartoffice_web.WebApi.Repositories;

namespace smartoffice_web.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameObjectController : ControllerBase
    {
        private readonly IGameObjectRepository _gameObjectRepository;
        private readonly ILogger<GameObjectController> _logger;

        public GameObjectController(IGameObjectRepository gameObjectRepository, ILogger<GameObjectController> logger)
        {
            _gameObjectRepository = gameObjectRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<GameObject>> Get()
        {
            _logger.LogInformation("Fetching all game objects.");
            return await _gameObjectRepository.GetAllGameObjectsAsync();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GameObject>> GetById(Guid id)
        {
            _logger.LogInformation($"Fetching game object with ID: {id}");
            var gameObject = await _gameObjectRepository.GetGameObjectByIdAsync(id);
            if (gameObject == null)
            {
                _logger.LogWarning($"Game object with ID {id} not found.");
                return NotFound();
            }
            return Ok(gameObject);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GameObject gameObject)
        {
            if (gameObject == null)
            {
                _logger.LogWarning("Attempted to create a null game object.");
                return BadRequest("Invalid game object data.");
            }

            gameObject.Id = gameObject.Id == Guid.Empty ? Guid.NewGuid() : gameObject.Id;
            _logger.LogInformation($"Creating game object with ID: {gameObject.Id}");

            await _gameObjectRepository.AddGameObjectAsync(gameObject);
            return CreatedAtAction(nameof(GetById), new { id = gameObject.Id }, gameObject);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] GameObject gameObject)
        {
            if (gameObject == null || id != gameObject.Id)
            {
                _logger.LogWarning("Game object ID mismatch or invalid data provided.");
                return BadRequest("Invalid game object data.");
            }

            _logger.LogInformation($"Updating game object with ID: {id}");
            await _gameObjectRepository.UpdateGameObjectAsync(gameObject);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation($"Deleting game object with ID: {id}");
            await _gameObjectRepository.DeleteGameObjectAsync(id);
            return NoContent();
        }
    }
}
