
using CutOutWiz.Data;
using CutOutWiz.Data.Security;

namespace CutOutWiz.Services.Security
{
    public interface IUserService
    {
        Task<User> GetUserByUsername(string username);
        Task<Response<bool>> Delete(string objectId);
        Task<List<User>> GetAll();
        Task<User> GetById(int userId);
        Task<User> GetByObjectId(string objectId);
        Task<Response<int>> Insert(User user);
        Task<Response<bool>> Update(User user);


        /// <summary>
        /// Hash a password using a random salt.
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        string GetHashPassword(string pass, string salt);
        string CreateRandomSalt();

        #region User Roles
        /// <summary>
        /// Get Roles by User object Id
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<List<string>> GetRolesByUserObjectId(string userObjectId);


        /// <summary>
        /// Insert user roles by user object id
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<Response<bool>> UserRoleInsertOrUpdateByUserObjectId(string userObjectId, List<string> roleObjectIds, int updatedByContactId);
        
        #endregion
    }
}