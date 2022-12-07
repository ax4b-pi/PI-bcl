using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using PIBcl.Cqrs.Models;
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

        public async Task<bool> ManipulationEntityList(List<AssignParamsManipulationEntity> objects)
        {
            using (var connectionMethod = new MySqlConnection(_connectionString))
            {
                int entity = 0;

                connectionMethod.Open();

                using (var transaction = connectionMethod.BeginTransaction())
                {
                    foreach (var objectCurrent in objects)
                    {
                        try
                        {
                            await connectionMethod.ExecuteAsync(
                                objectCurrent.Query,
                                objectCurrent.Params,
                                transaction: transaction
                            );

                            entity++;
                        }
                        catch (System.Exception ex)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }
                    if (objects.Count == entity)
                    {
                        transaction.Commit();
                        return entity != 0;
                    }
                    else
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

                connectionMethod.Close();
            }
        }
    }
}
