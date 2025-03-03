using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using smartoffice_web.WebApi.Models;
using smartoffice_web.WebApi.Repositories;
using smartoffice_web.WebApi.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace smartoffice_web.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IIdentityService _identityService;
        private readonly ILogger<AppUserController> _logger;

        public AppUserController(IAppUserRepository appUserRepository, IIdentityService identityService, ILogger<AppUserController> logger)
        {
            _appUserRepository = appUserRepository;
            _identityService = identityService;
            _logger = logger;
        }

        [HttpGet("identity/{identityUserId}")]
        public async Task<ActionResult<AppUser>> GetByIdentityUserId(string identityUserId)
        {
            _logger.LogInformation("Fetching user with IdentityUserId: {IdentityUserId}", identityUserId);
            var user = await _appUserRepository.GetByIdentityUserIdAsync(identityUserId);
            if (user == null)
            {
                _logger.LogWarning("User with IdentityUserId {IdentityUserId} not found.", identityUserId);
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("current")]
        public async Task<ActionResult<Guid>> GetCurrentUser()
        {
            try
            {
                var identityUserId = await _identityService.GetCurrentUserIdAsync(User);
                _logger.LogInformation("Fetching current user with IdentityUserId: {IdentityUserId}", identityUserId);

                var user = await _appUserRepository.GetByIdentityUserIdAsync(identityUserId);
                if (user == null)
                {
                    _logger.LogWarning("Current user with IdentityUserId {IdentityUserId} not found.", identityUserId);
                    return NotFound();
                }
                return Ok(user.Id);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access while fetching current user.");
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] AppUser user)
        {
            if (user == null)
            {
                _logger.LogWarning("Attempted to create a user with invalid data.");
                return BadRequest("Invalid user data.");
            }
            user.Id = Guid.NewGuid();
            _logger.LogInformation("Creating new user with Id: {UserId}", user.Id);
            var createdUserId = await _appUserRepository.CreateAppUserAsync(user);
            return CreatedAtAction(nameof(GetByIdentityUserId), new { identityUserId = user.IdentityUserId }, createdUserId);
        }
        
        [HttpGet("worlds/{userId}")]
        [Authorize]
        public async Task<ActionResult<Guid>> GetUserWorlds(Guid userId)
        {
            _logger.LogInformation("Fetching worlds for user with Id: {UserId}", userId);
            var userWorlds = await _appUserRepository.GetUserWorlds(userId);
            if (userWorlds == null)
            {
                _logger.LogWarning("Worlds for user with Id {UserId} not found.", userId);
                return NotFound();
            }
            _logger.LogInformation("Worlds for user with Id {UserId} found.", userId);
            _logger.LogInformation("Worlds found: {userWorlds}", userWorlds);
            return Ok(userWorlds);
        }
    }
}
