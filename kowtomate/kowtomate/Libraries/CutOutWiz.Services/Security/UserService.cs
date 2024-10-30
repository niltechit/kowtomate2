using CutOutWiz.Data;
using CutOutWiz.Data.Security;
using CutOutWiz.Services.DbAccess;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;

namespace CutOutWiz.Services.Security
{
    public class UserService : IUserService
    {
        private readonly ISqlDataAccess _db;

        public UserService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All Users
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<User, dynamic>(storedProcedure: "dbo.SP_Security_User_GetAll", new { });
        }

        /// <summary>
        /// Get user by user Id
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<User> GetById(int userId)
        {
            var result = await _db.LoadDataUsingProcedure<User, dynamic>(storedProcedure: "dbo.SP_Security_User_GetById", new { UserId = userId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<User> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<User, dynamic>(storedProcedure: "dbo.SP_Security_User_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(User user)
        {
            var response = new Response<int>();

            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<int, dynamic>(storedProcedure: "dbo.SP_Security_User_Insert", new
                {
                    user.CompanyId,
                    user.ContactId,
                    user.Username,
                    user.ProfileImageUrl,
                    user.PasswordHash,
                    user.PasswordSalt,
                    user.RegistrationToken,
                    user.PasswordResetToken,
                    user.LastLoginDateUtc,
                    user.LastLogoutDateUtc,
                    user.LastPasswordChangeUtc,
                    user.Status,
                    user.CreatedByContactId,
                    user.ObjectId,
                });

                user.Id = (short)newId;
                response.Result = newId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(User user)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_User_Update", new
                {
                    user.Id,
                    user.CompanyId,
                    user.ContactId,
                    user.Username,
                    user.ProfileImageUrl,
                    user.PasswordHash,
                    user.PasswordSalt,
                    user.RegistrationToken,
                    user.PasswordResetToken,
                    user.LastLoginDateUtc,
                    user.LastLogoutDateUtc,
                    user.LastPasswordChangeUtc,
                    user.Status,
                    user.UpdatedByContactId
                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Delete User by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_User_Delete", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var results = await _db.LoadDataUsingProcedure<User, dynamic>(storedProcedure: "dbo.SP_Security_User_GetByUsername", new { Username = username });

            return results.FirstOrDefault();
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

        #region User Roles
        /// <summary>
        /// Get Roles by User object Id
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<List<string>> GetRolesByUserObjectId(string userObjectId)
        {
            var query = "SELECT RoleObjectId FROM [dbo].[Security_UserRole] p WITH(NOLOCK) WHERE p.UserObjectId = @UserObjectId";
            
            return await _db.LoadDataUsingQuery<string, dynamic>(query, new { UserObjectId = userObjectId });
        }

        /// <summary>
        /// Insert user roles by user object id
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<Response<bool>> UserRoleInsertOrUpdateByUserObjectId(string userObjectId, List<string> roleObjectIds, int updatedByContactId)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_UserRole_InsertOrUpdateByUserObjectId", new
                {
                    UserObjectId = userObjectId,
                    RoleObjectIds = string.Join(",", roleObjectIds),
                    UpdatedByContactId = updatedByContactId
                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }
        #endregion
    }
}
