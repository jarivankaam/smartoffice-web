using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using smartoffice_web.WebApi.Models;
using smartoffice_web.WebApi.Repositories;
using smartoffice_web.WebApi.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace smartoffice_web.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IIdentityService _identityService;

        public AppUserController(IAppUserRepository appUserRepository, IIdentityService identityService)
        {
            _appUserRepository = appUserRepository;
            _identityService = identityService;
        }

        [HttpGet("identity/{identityUserId}")]
        public async Task<ActionResult<AppUser>> GetByIdentityUserId(string identityUserId)
        {
            var user = await _appUserRepository.GetByIdentityUserIdAsync(identityUserId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<AppUser>> GetUserId(string userId)
        {
            var user = await _appUserRepository.GetUserIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("current")] 
        public async Task<ActionResult<AppUser>> GetCurrentUser()
        {
            try
            {
                var identityUserId = await _identityService.GetCurrentUserIdAsync(User);
                var user = await _appUserRepository.GetByIdentityUserIdAsync(identityUserId);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] AppUser user)
        {
            if (user == null)
            {
                return BadRequest("Invalid user data.");
            }
            user.Id = Guid.NewGuid();
            var createdUserId = await _appUserRepository.CreateAppUserAsync(user);
            return CreatedAtAction(nameof(GetUserId), new { userId = createdUserId }, createdUserId);
        }
    }
}
