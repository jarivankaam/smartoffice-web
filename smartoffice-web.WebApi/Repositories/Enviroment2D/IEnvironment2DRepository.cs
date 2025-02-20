using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using smartoffice_web.WebApi.Models;

namespace smartoffice_web.WebApi.Repositories
{
    public interface IEnviroment2DRepository
    {
        Task<IEnumerable<Enviroment2D>> GetAllWorldsAsync();
        Task<Enviroment2D?> GetWorldByIdAsync(Guid id);  // Changed int to Guid
        Task AddWorldAsync(Enviroment2D enviroment2D);
        Task UpdateWorldAsync(Enviroment2D enviroment2D);
        Task DeleteWorldAsync(Guid id);  // Changed int to Guid
    }
}