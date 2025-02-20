using Dapper;
using smartoffice_web.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace smartoffice_web.WebApi.Repositories
{
    public class Environment2DRepository : IEnvironment2DRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<Environment2DRepository> _logger;

        public Environment2DRepository(IDbConnection dbConnection, ILogger<Environment2DRepository> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public async Task<IEnumerable<Environment2D>> GetAllEnvironment2DsAsync()
        {
            try
            {
                _logger.LogInformation("🔍 Executing query: SELECT * FROM Environment2D");

                string sql = "SELECT id, name, maxHeight, maxWidth FROM Environment2D";
                var result = await _dbConnection.QueryAsync<Environment2D>(sql);

                _logger.LogInformation($"✅ Query executed successfully. Retrieved {result.AsList().Count} Environment2D.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ ERROR in GetAllEnvironment2DsAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Environment2D?> GetWorldByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation($"🔍 Fetching world with ID: {id}");

                string sql = "SELECT id, name, maxHeight, maxWidth FROM Environment2D WHERE id = @Id";
                var world = await _dbConnection.QueryFirstOrDefaultAsync<Environment2D>(sql, new { Id = id });

                if (world != null)
                    _logger.LogInformation("✅ World retrieved successfully.");
                else
                    _logger.LogWarning($"⚠️ No world found with ID: {id}");

                return world;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ ERROR in GetWorldByIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task AddWorldAsync(Environment2D environment2D)
        {
            try
            {
                environment2D.Id = environment2D.Id == Guid.Empty ? Guid.NewGuid() : environment2D.Id;
                _logger.LogInformation($"📝 Inserting new world with ID: {environment2D.Id}");

                string sql = "INSERT INTO Environment2D (id, name, maxHeight, maxWidth) VALUES (@Id, @Name, @MaxHeight, @MaxWidth)";
                await _dbConnection.ExecuteAsync(sql, environment2D);

                _logger.LogInformation("✅ World inserted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ ERROR in AddWorldAsync: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateWorldAsync(Environment2D environment2D)
        {
            try
            {
                _logger.LogInformation($"🔄 Updating world with ID: {environment2D.Id}");

                string sql = "UPDATE Environment2D SET name = @Name, maxHeight = @MaxHeight, maxWidth = @MaxWidth WHERE id = @Id";
                await _dbConnection.ExecuteAsync(sql, environment2D);

                _logger.LogInformation("✅ World updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ ERROR in UpdateWorldAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteWorldAsync(Guid id)
        {
            try
            {
                _logger.LogInformation($"🗑 Deleting world with ID: {id}");

                string sql = "DELETE FROM Environment2D WHERE id = @Id";
                await _dbConnection.ExecuteAsync(sql, new { Id = id });

                _logger.LogInformation("✅ World deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ ERROR in DeleteWorldAsync: {ex.Message}");
                throw;
            }
        }
    }
}
