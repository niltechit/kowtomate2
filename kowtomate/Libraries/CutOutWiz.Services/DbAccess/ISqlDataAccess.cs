using System.Data;

namespace CutOutWiz.Services.DbAccess
{
    public interface ISqlDataAccess
    {
        Task<List<T>> LoadDataUsingProcedure<T, U>(string storedProcedure, U paramiters, string connectionId = "Default");
        Task<List<T>> LoadDataUsingQuery<T, U>(string sqlQuery, U paramiters, string connectionId = "Default");
        Task SaveDataSetUsingProcedure(string storedProcedure, DataTable dataTable, string typeName = "", string connectionId = "Default");
        Task SaveDataUsingProcedure<T>(string storedProcedure, T paramiters, string connectionId = "Default");
        Task<T> SaveDataUsingProcedureAndReturnId<T, U>(string storedProcedure, U paramiters, string connectionId = "Default");
        Task<int> CountDataUsingProcedure(string storedProcedure, string connectionId = "Default");
        Task SaveDataUsingQuery<T>(string sqlQuery, T paramiters, string connectionId = "Default");
        Task<T> SaveDataUsingQueryAndReturnId<T, U>(string sqlQuery, U paramiters, string connectionId = "Default");
        Task<int> SaveDataUsingProcedureAndReturnNumberOfEffectedRow<T>(string storedProcedure, T paramiters, string connectionId = "Default");

        //Task<IEnumerable<IDictionary<string, object>>> LoadDictionaryDataUsingQuery<U>(string sqlQuery,
        //   U paramiters, string connectionId = "Default");

         Task<IEnumerable<IDictionary<string, object>>> LoadDictionaryDataUsingQuery<U>(string sqlQuery,
           U paramiters, string connectionId = "Default");

          Task<IEnumerable<IDictionary<string, object>>> LoadDictionaryDataUsingProcedure<U>(string storedProcedure,
           U paramiters, string connectionId = "Default");
        

        Task<int> GetDataUsingProcedure<T, U>(string storedProcedure, U paramiters, string connectionId = "Default");

        Task<T> LoadFirstOrDefaultDataUsingQuery<T, U>(string sqlQuery,
           U paramiters, string connectionId = "Default");

        Task<int> SaveDataUsingProcedureWithGeneric<T>(string storedProcedure,T model,string connectionId = "Default");
        Task<bool> UpdateDataUsingProcedureWithGeneric<T>(string storedProcedure, T model, string connectionId = "Default");

    }
}