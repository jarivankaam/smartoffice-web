using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using smartoffice_web.WebApi.Models;

namespace smartoffice_web.WebApi.Repositories
{
    public interface IWorldRepository
    {
        Task<IEnumerable<World>> GetAllWorldsAsync();
        Task<World?> GetWorldByIdAsync(Guid id);  // Changed int to Guid
        Task AddWorldAsync(World world);
        Task UpdateWorldAsync(World world);
        Task DeleteWorldAsync(Guid id);  // Changed int to Guid
    }
}