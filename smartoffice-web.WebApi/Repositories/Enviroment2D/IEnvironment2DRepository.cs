using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using smartoffice_web.WebApi.Models;

namespace smartoffice_web.WebApi.Repositories
{
    public interface IEnvironment2DRepository
    {
        Task<IEnumerable<Environment2D>> GetAllEnvironment2DsAsync();
        Task<Environment2D?> GetWorldByIdAsync(Guid id);  // Changed int to Guid
        Task AddWorldAsync(Environment2D environment2D);
        Task UpdateWorldAsync(Environment2D environment2D);
        Task DeleteWorldAsync(Guid id);  // Changed int to Guid
    }
}