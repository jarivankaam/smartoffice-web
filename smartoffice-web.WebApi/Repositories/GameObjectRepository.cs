using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using smartoffice_web.WebApi.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace smartoffice_web.WebApi.Repositories
{
    public class GameObjectRepository : IGameObjectRepository
    {
        private readonly IDbConnection _dbConnection;

        public GameObjectRepository(IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");
            _dbConnection = new SqlConnection(connectionString);
        }

        public async Task<IEnumerable<GameObject>> GetAllGameObjectsAsync()
        {
            string sql = "SELECT id, prefabId, positionX, positionY, scaleX, scaleY, RotationZ, SortingLayer, worldId FROM objects";
            return await _dbConnection.QueryAsync<GameObject>(sql);
        }

        public async Task<GameObject?> GetGameObjectByIdAsync(Guid id)
        {
            string sql = "SELECT id, prefabId, positionX, positionY, scaleX, scaleY, RotationZ, SortingLayer, worldId FROM objects WHERE id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<GameObject>(sql, new { Id = id });
        }

        public async Task AddGameObjectAsync(GameObject gameObject)
        {
            string sql = @"
                INSERT INTO objects (id, prefabId, positionX, positionY, scaleX, scaleY, RotationZ, SortingLayer, worldId) 
                VALUES (@Id, @PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer, @WorldId)";

            gameObject.Id = gameObject.Id == Guid.Empty ? Guid.NewGuid() : gameObject.Id; // Ensure a GUID is assigned
            await _dbConnection.ExecuteAsync(sql, gameObject);
        }

        public async Task UpdateGameObjectAsync(GameObject gameObject)
        {
            string sql = @"
                UPDATE objects 
                SET prefabId = @PrefabId, positionX = @PositionX, positionY = @PositionY, 
                    scaleX = @ScaleX, scaleY = @ScaleY, RotationZ = @RotationZ, SortingLayer = @SortingLayer, 
                    worldId = @WorldId 
                WHERE id = @Id";

            await _dbConnection.ExecuteAsync(sql, gameObject);
        }

        public async Task DeleteGameObjectAsync(Guid id)
        {
            string sql = "DELETE FROM objects WHERE id = @Id";
            await _dbConnection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
