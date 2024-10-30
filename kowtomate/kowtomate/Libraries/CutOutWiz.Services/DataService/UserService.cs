using CutOutWiz.Data;
using CutOutWiz.Data.Security;
using CutOutWiz.Services.DbAccess;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;

namespace CutOutWiz.Services.DataService
{
    public class UserService : IUserService
    {
        private readonly ISqlDataAccess _db;
        private readonly IMemoryCache _memoryCache;
        public UserService(ISqlDataAccess db, IMemoryCache memoryCache)
        {
            _db = db;
            _memoryCache = memoryCache;
        }

        public async Task<IEnumerable<UserModel>> GetUsers()
        {
            return await _db.LoadDataUsingProcedure<UserModel, dynamic>(storedProcedure: "dbo.spUser_GetAll", new { });
        }

        public async Task<IEnumerable<UserViewModel>> GetAllUserDetails()
        {
            return await _db.LoadDataUsingProcedure<UserViewModel, dynamic>(storedProcedure: "dbo.sp_User_GetAllUser", new { });
            
        }

        public async Task<IEnumerable<Data.Company>> GetAllCompany()
        {
            return await _db.LoadDataUsingProcedure<Data.Company, dynamic>(storedProcedure: "dbo.spCompany_GetAll", new { });

        }
        public async Task<IEnumerable<Data.Role>> GetAllRoles()
        {
            return await _db.LoadDataUsingProcedure<Data.Role, dynamic>(storedProcedure: "dbo.spRole_GetAll", new { });

        }
        public async Task<LastInsertedId> GetLastInsertedId(string tableName)
        {
            var results = await _db.LoadDataUsingProcedure<LastInsertedId, dynamic>(
                storedProcedure: "dbo.spGetLastInsertedId",
                new { tableName = tableName });

            return  results.FirstOrDefault();
        }



        public async Task<UserModel?> GetUser(int id)
        {
            var results = await _db.LoadDataUsingProcedure<UserModel, dynamic>(
                storedProcedure: "dbo.spUser_Get",
                new { Id = id });

            return results.FirstOrDefault();
        }

        public async Task<UserModel?> GetUserByUsername(string username)
        {
            var results = await _db.LoadDataUsingProcedure<UserModel, dynamic>(
                storedProcedure: "dbo.spUser_GetByUsername",
                new { Username = username });

            return results.FirstOrDefault();
        }

        public async Task<UserViewModel?> GetUserByUsernameForUpdate(string username)
        {
            var results = await _db.LoadDataUsingProcedure<UserViewModel, dynamic>(
                storedProcedure: "dbo.spUser_GetByUsernameForUpdate",
                new { Username = username });

            return results.FirstOrDefault();
        }


        public async Task InsertUser(UserModel user)
        {
            try
            {                
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.spUser_Insert", new
                {
                    ContactId = user.ContactId,
                    RoleId = user.RoleId,
                    Username = user.Username,
                    PasswordHash = user.PasswordHash,
                    PasswordSalt = user.PasswordSalt,
                    UserGuid = user.UserGuid,
                    Status = user.Status
                });
            }
            catch (Exception ex)
            {
                string mes = ex.Message;
            }
           
        }
            
        public async Task<int> InsertContact(Data.Contact contact)
        {
            try
            {               
               var response = await _db.SaveDataUsingProcedureAndReturnId<int,dynamic>(storedProcedure: "dbo.spContact_Insert", new
                {
                    CompanyId = contact.CompanyId,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    Email = contact.Email,
                    Phone = contact.Phone,
                    ContactGuid = contact.ContactGuid

                });
                return response;
            }
            catch (Exception ex)
            {
                string mes = ex.Message;
            }
            return 0;
        }
        public async Task<int> UpdateUser(UserViewModel user)
        {
            try
            {
                var response = await _db.SaveDataUsingProcedureAndReturnId<int, dynamic>(storedProcedure: "dbo.spUser_Update", new
                {
                    UserId = user.UserId,
                    ContactId = user.ContactId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    CompanyId = user.CompanyName,
                    Phone = user.Phone,
                    RoleId = user.RoleName
                });
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public Task DeleteUser(int id) =>
            _db.SaveDataUsingProcedure(storedProcedure: "dbo.spUser_Delete", new { Id = id });


        public async Task<UserDetailInfoModel?> GetLoginUserInfo(string userGuid, bool isCache = false)
        {
            UserDetailInfoModel? userInfo = null;

            if (isCache)
            {
                userInfo = _memoryCache.Get<UserDetailInfoModel>(key: $"loginuserinfo{userGuid}");
            }

            if (userInfo is null)
            {
                var result = await _db.LoadDataUsingProcedure<UserDetailInfoModel, dynamic>(
                storedProcedure: "dbo.sp_User_GeLoginUserInfo",
                new { @userGuid = userGuid });

                userInfo = result.FirstOrDefault();

                if (isCache)
                {
                    _memoryCache.Set<UserDetailInfoModel?>(key: $"loginuserinfo{userGuid}", userInfo, TimeSpan.FromMinutes(value: 120));
                }
            }

            return userInfo;
        }

        /// <summary>
        /// Hash a password using a random salt.
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public string GetHashPassword(string pass, string salt)
        {
            var bytes = Encoding.Unicode.GetBytes(pass);
            var src = Encoding.Unicode.GetBytes(salt)
;
            var dst = new byte[src.Length + bytes.Length];
            Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            var algorithm = HashAlgorithm.Create("SHA1");

            if (algorithm == null)
                return String.Empty;

            var inArray = algorithm.ComputeHash(dst);
            return Convert.ToBase64String(inArray);
        }
        /// <summary>
        /// Creates a random password salt
        /// </summary>
        /// <returns></returns>
        public string CreateRandomSalt()
        {
            var saltBytes = new Byte[4];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        #region Private Methos

        #endregion

    }
}
