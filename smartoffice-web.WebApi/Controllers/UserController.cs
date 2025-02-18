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
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            _logger.LogInformation("Fetching all users.");
            return await _userRepository.GetAllUsersAsync();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<User>> GetById(Guid id)
        {
            _logger.LogInformation($"Fetching user with ID: {id}");
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} not found.");
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (user == null)
            {
                _logger.LogWarning("Attempted to create a null user.");
                return BadRequest("Invalid user data.");
            }

            user.Id = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id;
            _logger.LogInformation($"Creating user with ID: {user.Id}");

            await _userRepository.AddUserAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] User user)
        {
            if (user == null || id != user.Id)
            {
                _logger.LogWarning("User ID mismatch or invalid data provided.");
                return BadRequest("Invalid user data.");
            }

            _logger.LogInformation($"Updating user with ID: {id}");
            await _userRepository.UpdateUserAsync(user);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation($"Deleting user with ID: {id}");
            await _userRepository.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
