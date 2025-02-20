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
    public class Object2DRepository : IObject2DRepository
    {
        private readonly IDbConnection _dbConnection;

        public Object2DRepository(IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");
            _dbConnection = new SqlConnection(connectionString);
        }

        public async Task<IEnumerable<Object2D>> GetAllObject2DsAsync()
        {
            string sql = "SELECT id, prefabId, positionX, positionY, scaleX, scaleY, RotationZ, SortingLayer, worldId FROM Object2D";
            return await _dbConnection.QueryAsync<Object2D>(sql);
        }

        public async Task<Object2D?> GetObject2DByIdAsync(Guid id)
        {
            string sql = "SELECT id, prefabId, positionX, positionY, scaleX, scaleY, RotationZ, SortingLayer, worldId FROM Object2D WHERE id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Object2D>(sql, new { Id = id });
        }

        public async Task AddObject2DAsync(Object2D Object2D)
        {
            string sql = @"
                INSERT INTO Object2D (id, prefabId, positionX, positionY, scaleX, scaleY, RotationZ, SortingLayer, worldId) 
                VALUES (@Id, @PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer, @WorldId)";

            Object2D.Id = Object2D.Id == Guid.Empty ? Guid.NewGuid() : Object2D.Id; // Ensure a GUID is assigned
            await _dbConnection.ExecuteAsync(sql, Object2D);
        }

        public async Task UpdateObject2DAsync(Object2D Object2D)
        {
            string sql = @"
                UPDATE Object2D 
                SET prefabId = @PrefabId, positionX = @PositionX, positionY = @PositionY, 
                    scaleX = @ScaleX, scaleY = @ScaleY, RotationZ = @RotationZ, SortingLayer = @SortingLayer, 
                    worldId = @WorldId 
                WHERE id = @Id";

            await _dbConnection.ExecuteAsync(sql, Object2D);
        }

        public async Task DeleteObject2DAsync(Guid id)
        {
            string sql = "DELETE FROM Object2D WHERE id = @Id";
            await _dbConnection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
