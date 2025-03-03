using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using smartoffice_web.WebApi.Models;
using smartoffice_web.WebApi.Repositories;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IAppUserRepository _appUserRepository;

    public AuthController(UserManager<IdentityUser> userManager, IAppUserRepository appUserRepository)
    {
        _userManager = userManager;
        _appUserRepository = appUserRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Create the IdentityUser
        var identityUser = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email,
        };

        var result = await _userManager.CreateAsync(identityUser, model.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        // Create the associated AppUser record
        var appUser = new AppUser
        {
            IdentityUserId = identityUser.Id,
            DisplayName = model.DisplayName
        };

        await _appUserRepository.AddAsync(appUser);

        return Ok("User registered successfully.");
    }
}
