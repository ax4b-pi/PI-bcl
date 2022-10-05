using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PIBcl.Cqrs.Querys
{
    public class DatabaseConnection<T> where T : class
    {

        private readonly MySqlConnection _connection;
        private IConfiguration _configuration;
        private string _connectionString { get { return _configuration.GetConnectionString("DefaultConnection"); } }

        public DatabaseConnection(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new MySqlConnection(_connectionString);
        }

        public async Task<IEnumerable<T>> QueryEntity(string query, object parameters = null)
        {
            using (_connection)
            {
                IEnumerable<T> entity;

                try
                {
                    entity = await _connection.QueryAsync<T>(query, parameters);
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
