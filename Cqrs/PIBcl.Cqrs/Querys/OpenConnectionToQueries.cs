using Dapper;
using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PIBcl.Cqrs.Querys
{
    public class OpenConnectionToQueries<T> where T : class
    {
        public static async Task<IEnumerable<T>> QueryEntity(string _connection, string query, string conditionQuery = "", object parameters = null) 
        {
            MySqlConnection mySqlConnection = new MySqlConnection(_connection);
            using (var connection = mySqlConnection)
            {
                IEnumerable<T> entity;

                try
                {
                    entity = await connection.QueryAsync<T>(query+" " + conditionQuery, parameters);
                }
                catch (System.Exception ex)
                {
                    return null;
                }
                return entity;
            }
        }
        public static async Task<IEnumerable<T>> ManipulationEntityToDatabase(string _connection, string query, string conditionQuery = "", object parameters = null)
        {
            MySqlConnection mySqlConnection = new MySqlConnection(_connection);
            using (var connection = mySqlConnection)
            {
                IEnumerable<T> entity;

                try
                {
                    entity = await connection.QueryAsync<T>("" + query + " " + conditionQuery, parameters);
                }
                catch (System.Exception ex)
                {
                    return null;
                }
                return entity;
            }
        }
    }
}
