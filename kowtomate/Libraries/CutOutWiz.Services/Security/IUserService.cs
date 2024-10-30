
using CutOutWiz.Core;
using CutOutWiz.Services.Models.Security;
using System.Security.Claims;

namespace CutOutWiz.Services.Security
{
    public interface IUserService
    {
        Task<UserModel> GetUserByUsername(string username);
        Task<Response<bool>> Delete(string objectId);
        Task<List<UserModel>> GetAll();
        Task<UserModel> GetById(int userId);
        Task<UserModel> GetByObjectId(string objectId);
        Task<Response<int>> Insert(UserModel user);
        Task<Response<bool>> Update(UserModel user);
        Task<UserModel> GetUserByContactId(int contactId);
        Task<UserModel> GetUserByCompanyId(int companyId);

        /// <summary>
        /// Hash a password using a random salt.
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        string GetHashPassword(string pass, string salt);
        string CreateRandomSalt();
        Task<Response<bool>> ResetPassword(UserViewModel user);
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
        Task<UserModel> GetUsername(string username);
        Task<bool> VerifyPassword(string providedPassword, string storedPasswordHash, string storedPasswordSalt);
		Task<Response<UserModel>> ValidateUserPassword(UserViewModel userInfo);
        Task<Response<bool>> ChangePassword(UserViewModel userInfo, Claim? claim = null);
	}
}