using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using PIBcl.Cqrs.Exceptions;
using PIBcl.Cqrs.Models;
using System;
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
                catch (Exception ex)
                {
                    throw ex;
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
                catch (Exception ex)
                {
                    throw ex;
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
                            await connectionMethod.ExecuteAsync(objectCurrent.Query,
                                objectCurrent.Params, transaction: transaction);

                            entity++;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            connectionMethod.Close();
                            throw ex;
                        }
                    }
                    if (objects.Count == entity)
                    {
                        transaction.Commit();
                        connectionMethod.Close();
                        return entity != 0;
                    }
                    else
                    {
                        transaction.Rollback();
                        connectionMethod.Close();
                        return false;
                    }
                }
            }
        }
        public async Task<bool> ExecuteList(List<List<AssignParamsManipulationEntity>> objects)
        {

            using (var connectionMethod = new MySqlConnection(_connectionString))
            {
                int entity = 0;
                connectionMethod.Open();
                using (var transaction = connectionMethod.BeginTransaction())
                {
                    foreach (var aba in objects)
                    {
                        foreach (var objectCurrent in aba)
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
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                connectionMethod.Close();
                                throw ex;
                            }
                        }
                    }

                    transaction.Commit();
                    connectionMethod.Close();
                    return true;

                }

            }
        }
        public MySqlTransaction IniciateTransaction()
        {
            using (var connectionMethod = new MySqlConnection(_connectionString))
            {
                connectionMethod.Open();
                MySqlTransaction transaction = connectionMethod.BeginTransaction();
                connectionMethod.Close();
                return transaction;
            }
        }
        public bool CompleteTransaction(MySqlTransaction transaction)
        {
            using (var connectionMethod = new MySqlConnection(_connectionString))
            {
                try
                {
                    connectionMethod.Open();
                    transaction.Commit();
                    connectionMethod.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
