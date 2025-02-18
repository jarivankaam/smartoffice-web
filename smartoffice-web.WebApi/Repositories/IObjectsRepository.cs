using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using smartoffice_web.WebApi.Models;

namespace smartoffice_web.WebApi.Repositories
{
    public interface IGameObjectRepository
    {
        Task<IEnumerable<GameObject>> GetAllGameObjectsAsync();
        Task<GameObject?> GetGameObjectByIdAsync(Guid id);
        Task AddGameObjectAsync(GameObject gameObject);
        Task UpdateGameObjectAsync(GameObject gameObject);
        Task DeleteGameObjectAsync(Guid id);
    }
}