using CutOutWiz.Data;
using CutOutWiz.Data.Security;

namespace CutOutWiz.Services.DataService
{
    public interface IUserService
    {
        Task DeleteUser(int id);
        Task<UserModel?> GetUser(int id);
        Task<UserModel?> GetUserByUsername(string username);
        Task<int> UpdateUser(UserViewModel user);
        Task<UserViewModel?> GetUserByUsernameForUpdate(string username);
        Task<IEnumerable<UserModel>> GetUsers();
        Task<IEnumerable<UserViewModel>> GetAllUserDetails();
        Task InsertUser(UserModel user);
        Task<UserDetailInfoModel?> GetLoginUserInfo(string userGuid, bool isCache = false);
        Task<IEnumerable<Data.Company>> GetAllCompany();
        Task<IEnumerable<Data.Role>> GetAllRoles();
        Task<LastInsertedId> GetLastInsertedId(string tableName);
        Task<int> InsertContact(Data.Contact contact);
        /// <summary>
        /// Hash a password using a random salt.
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        string GetHashPassword(string pass, string salt);
        string CreateRandomSalt();
    }
}