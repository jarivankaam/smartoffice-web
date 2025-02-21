using System.Security.Claims;
using System.Threading.Tasks;

namespace smartoffice_web.WebApi.Services
{
    public interface IIdentityService
    {
        Task<string> GetCurrentUserIdAsync(ClaimsPrincipal user);
    }
}