using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PIBcl.Cqrs.Querys
{
    public class DatabaseConnection
    {
        private IConfiguration _configuration;
        private string _connectionString { get { return _configuration.GetConnectionString("DefaultConnection"); } }

        public DatabaseConnection(IConfiguration configuration)
        {
            _configuration = configuration;        
        }

        public async Task<IEnumerable<T>> QueryEntity<T>(string query, object parameters = null)
        {
            using (var connectionMethod = new MySqlConnection(_connectionString))
            {
                IEnumerable<T> entity;

                try
                {
                    entity = await connectionMethod.QueryAsync<T>(query, parameters);
                }
                catch (System.Exception ex)
                {
                    return null;
                }
                return entity;
            }
        }
        public async Task<bool> ManipulationEntity(string query, object parameters = null)
        {
            using (var connectionMethod = new MySqlConnection(_connectionString))
            {
                int entity;

                try
                {
                    entity = await connectionMethod.ExecuteAsync(query, parameters);
                }
                catch (System.Exception ex)
                {
                    return false;
                }
                return entity != 0;
            }
        }
    }
}
