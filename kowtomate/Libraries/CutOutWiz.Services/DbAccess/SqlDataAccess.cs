using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace CutOutWiz.Services.DbAccess
{
    public class SqlDataAccess : ISqlDataAccess
    {
        private readonly IConfiguration _config;
        public SqlDataAccess(IConfiguration config)
        {
            _config = config;
        }

        #region Load Data Using Procedure
        public async Task<List<T>> LoadDataUsingProcedure<T, U>(string storedProcedure,
           U paramiters,
           string connectionId = "Default")
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));


                var data = await connection.QueryAsync<T>(storedProcedure, paramiters,
                    commandType: CommandType.StoredProcedure, commandTimeout: 120);

                return data.ToList();
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public async Task SaveDataUsingProcedure<T>(string storedProcedure,
           T paramiters,
           string connectionId = "Default")
        {
            try
            {

                using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

                await connection.ExecuteAsync(storedProcedure, paramiters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<int> SaveDataUsingProcedureAndReturnNumberOfEffectedRow<T>(string storedProcedure,
         T paramiters,
         string connectionId = "Default")
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

            return await connection.ExecuteAsync(storedProcedure, paramiters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<T> SaveDataUsingProcedureAndReturnId<T, U>(string storedProcedure,
          U paramiters,
          string connectionId = "Default")
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

            return await connection.ExecuteScalarAsync<T>(storedProcedure, paramiters,
                commandType: CommandType.StoredProcedure);
        }
        public async Task<int> CountDataUsingProcedure(string storedProcedure, string connectionId = "Default")
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

            return await connection.ExecuteScalarAsync<int>(storedProcedure,
                commandType: CommandType.StoredProcedure);
        }

        public async Task SaveDataSetUsingProcedure(string storedProcedure, DataTable dataTable,
            string typeName = "", string connectionId = "Default")
        {
            var p = new
            {
                items = dataTable.AsTableValuedParameter(typeName)
            };

            using (IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId)))
            {
                await connection.ExecuteAsync(storedProcedure, p,
                 commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> GetDataUsingProcedure<T, U>(string storedProcedure, U paramiters, string connectionId = "Default")
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId)))
                {
                    var data = await connection.QueryAsync<int>(storedProcedure, paramiters, commandType: CommandType.StoredProcedure);

                    return data.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        #endregion End load data using procedure

        #region Data Access using Query
        public async Task<List<T>> LoadDataUsingQuery<T, U>(string sqlQuery,
           U paramiters, string connectionId = "Default")
        {
            using (IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId)))
            {
                var data = await connection.QueryAsync<T>(sqlQuery, paramiters, commandTimeout: 220);

                return data.ToList();
            }
        }

        public async Task<T> LoadFirstOrDefaultDataUsingQuery<T, U>(string sqlQuery,
           U paramiters, string connectionId = "Default")
        {
            using (IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId)))
            {
                var data = await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, paramiters);

                return data;
            }
        }

        public async Task SaveDataUsingQuery<T>(string sqlQuery,
         T paramiters,
         string connectionId = "Default")
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

            await connection.ExecuteAsync(sqlQuery, paramiters);
        }

        public async Task<T> SaveDataUsingQueryAndReturnId<T, U>(string sqlQuery,
          U paramiters,
          string connectionId = "Default")
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
            return await connection.ExecuteScalarAsync<T>(sqlQuery, paramiters);
        }
        #endregion End Data Access using query

        #region Dictionary
        #region Dictionary
        public async Task<IEnumerable<IDictionary<string, object>>> LoadDictionaryDataUsingQuery<U>(string sqlQuery,
           U paramiters, string connectionId = "Default")
        {
            using (IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId)))
            {
                var tieout = connection.ConnectionTimeout; // Set to 60 seconds (or any other value suitable for your case)

                // Set the command timeout
                //var command = connection.CreateCommand();
                //command.CommandTimeout = 60; // Set to 60 seconds (or any other value suitable for your case)

                var tieout2 = connection.ConnectionTimeout; // Set to 60 seconds (or any other value suitable for your case)

                var data = await connection.QueryAsync(sqlQuery, paramiters, null, 200);

                //commandType: CommandType.StoredProcedure, transaction: null, commandTimeout: 240);

                var result = data.ToList();
                return result.Select(r => ((IDictionary<string, object>)r).ToDictionary(k => k.Key, k => k.Value));
            }
        }

        public async Task<IEnumerable<IDictionary<string, object>>> LoadDictionaryDataUsingProcedure<U>(string storedProcedure,
           U paramiters, string connectionId = "Default")
        {
            using (IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId)))
            {
                var tieout = connection.ConnectionTimeout; // Set to 60 seconds (or any other value suitable for your case)

                // Set the command timeout
                //var command = connection.CreateCommand();
                //command.CommandTimeout = 60; // Set to 60 seconds (or any other value suitable for your case)

                var tieout2 = connection.ConnectionTimeout; // Set to 60 seconds (or any other value suitable for your case)

                var data = await connection.QueryAsync(storedProcedure, paramiters, commandType: CommandType.StoredProcedure, transaction: null, commandTimeout: 240);

                //commandType: CommandType.StoredProcedure, transaction: null, commandTimeout: 240);

                var result = data.ToList();
                return result.Select(r => ((IDictionary<string, object>)r).ToDictionary(k => k.Key, k => k.Value));
            }
        }


        //public async Task<IEnumerable<IDictionary<string, object>>> LoadDictionaryDataUsingQuery<U>(string sqlQuery,
        //  U paramiters, string connectionId = "Default")
        //{
        //    using (IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId)))
        //    {
        //        var tieout = connection.ConnectionTimeout; // Set to 60 seconds (or any other value suitable for your case)

        //        // Set the command timeout
        //        //var command = connection.CreateCommand();
        //        //command.CommandTimeout = 60; // Set to 60 seconds (or any other value suitable for your case)

        //        var tieout2 = connection.ConnectionTimeout; // Set to 60 seconds (or any other value suitable for your case)

        //        var data = await connection.QueryAsync(sqlQuery, paramiters, null, 200);

        //        //commandType: CommandType.StoredProcedure, transaction: null, commandTimeout: 240);

        //        var result = data.ToList();
        //        return result.Select(r => ((IDictionary<string, object>)r).ToDictionary(k => k.Key, k => k.Value));
        //    }
        //}

        //public IEnumerable<IDictionary<string, object>> GetData()
        //{
        //    using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

        //    var data = await connection.QueryAsync<T>(storedProcedure, paramiters,
        //        commandType: CommandType.StoredProcedure);

        //    return data.ToList();
        //    string sql = "SELECT * FROM MyTable";
        //    var result = _db.Query(sql);

        //    return result.Select(r => ((IDictionary<string, object>)r).ToDictionary(k => k.Key, k => k.Value));
        //}
        #endregion 
        //public async Task<IEnumerable<IDictionary<string, object>>> LoadDictionaryDataUsingQuery<U>(string sqlQuery,
        //   U paramiters, string connectionId = "Default")
        //{
        //    using (IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId)))
        //    {
        //        var data = await connection.QueryAsync(sqlQuery, paramiters);

        //        var result = data.ToList();
        //        return result.Select(r => ((IDictionary<string, object>)r).ToDictionary(k => k.Key, k => k.Value));
        //    }
        //}

        //public IEnumerable<IDictionary<string, object>> GetData()
        //{
        //    using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

        //    var data = await connection.QueryAsync<T>(storedProcedure, paramiters,
        //        commandType: CommandType.StoredProcedure);

        //    return data.ToList();
        //    string sql = "SELECT * FROM MyTable";
        //    var result = _db.Query(sql);

        //    return result.Select(r => ((IDictionary<string, object>)r).ToDictionary(k => k.Key, k => k.Value));
        //}
        #endregion

        #region Generic Insert Method
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcedure"></param>
        /// <param name="model"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public async Task<int> SaveDataUsingProcedureWithGeneric<T>(string storedProcedure, T model, string connectionId = "Default")
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

            try
            {
                var parameters = new DynamicParameters();

                // Add parameters for all properties of the model, including nested ones
                AddParameters(parameters, model);

                // Add an output parameter to capture the ID
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

                // Retrieve the output parameter value
                int id = parameters.Get<int>("@Id");
                return id;
            }
            catch (SqlException sqlEx)
            {
                // Log or handle SQL-specific exceptions
                Console.WriteLine($"SQL Exception: {sqlEx.Message}");
                throw; // Re-throw the exception to let the caller handle it
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                Console.WriteLine($"Exception: {ex.Message}");
                throw; // Re-throw the exception to let the caller handle it
            }
        }

        private void AddParameters<T>(DynamicParameters parameters, T model, string prefix = "")
        {
            foreach (var prop in model.GetType().GetProperties())
            {
                var propValue = prop.GetValue(model);
                var paramName = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}_{prop.Name}";

                if (propValue != null && prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
                {
                    // Recursively add parameters for nested properties
                    AddParameters(parameters, propValue, paramName);
                }
                else
                {
                    parameters.Add(paramName, propValue);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcedure"></param>
        /// <param name="model"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateDataUsingProcedureWithGeneric<T>(string storedProcedure, T model, string connectionId = "Default")
        {
            bool response = false;
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

            try
            {
                var parameters = new DynamicParameters();

                // Add parameters for all properties of the model, including nested ones
                AddParameters(parameters, model);

                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

                response = true;
                
            }
            catch (SqlException sqlEx)
            {
                response = false;
                // Log or handle SQL-specific exceptions
                Console.WriteLine($"SQL Exception: {sqlEx.Message}");
                throw; // Re-throw the exception to let the caller handle it

            }
            catch (Exception ex)
            {
                response = false;
                // Log or handle other exceptions
                Console.WriteLine($"Exception: {ex.Message}");
                throw; // Re-throw the exception to let the caller handle it
            }
            return response;
        }

        #endregion Generic Insert Method
    }
}
