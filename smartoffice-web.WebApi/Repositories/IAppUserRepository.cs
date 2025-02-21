namespace smartoffice_web.WebApi.Repositories;
using smartoffice_web.WebApi.Models;
using System;
using System.Threading.Tasks;

public interface IAppUserRepository
{
    Task<AppUser> GetByIdentityUserIdAsync(string identityUserId);
    Task<Guid> CreateAppUserAsync(AppUser user);
}
