using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PIBcl.Cqrs.Querys
{
    public class ConnectionDatabase<T> where T : class
    {
        private readonly MySqlConnection _connection;
        public ConnectionDatabase(MySqlConnection connection)
        {
            _connection = connection;
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
            using (_connection)
            {
                try
                {                    
                    var entity = await _connection.ExecuteAsync(query, parameters);
                }
                catch (System.Exception ex)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
