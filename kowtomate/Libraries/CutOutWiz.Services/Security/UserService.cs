using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.DbAccess;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CutOutWiz.Data;


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
		public async Task<List<UserModel>> GetAll()
		{
			return await _db.LoadDataUsingProcedure<UserModel, dynamic>(storedProcedure: "dbo.SP_Security_User_GetAll", new { });
		}

		/// <summary>
		/// Get user by user Id
		/// </summary>
		/// <param name="UserId"></param>
		/// <returns></returns>
		public async Task<UserModel> GetById(int userId)
		{
			var result = await _db.LoadDataUsingProcedure<UserModel, dynamic>(storedProcedure: "dbo.SP_Security_User_GetById", new { UserId = userId });
			return result.FirstOrDefault();
		}

		/// <summary>
		/// Get by Object Id
		/// </summary>
		/// <param name="UserId"></param>
		/// <returns></returns>
		public async Task<UserModel> GetByObjectId(string objectId)
		{
			var result = await _db.LoadDataUsingProcedure<UserModel, dynamic>(storedProcedure: "dbo.SP_Security_User_GetByObjectId", new { ObjectId = objectId });
			return result.FirstOrDefault();
		}
		public async Task<UserModel> GetUserByContactId(int contactId)
		{
			var result = await _db.LoadDataUsingProcedure<UserModel, dynamic>(storedProcedure: "dbo.SP_Security_User_GetByContactId", new { ContactId = contactId });
			return result.FirstOrDefault();
		}
		public async Task<UserModel> GetUserByCompanyId(int companyId)
		{
			var result = await _db.LoadDataUsingProcedure<UserModel, dynamic>(storedProcedure: "dbo.SP_Security_User_GetByCompanyId", new { CompanyId = companyId });
			return result.FirstOrDefault();
		}
		/// <summary>
		/// Insert user
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public async Task<Response<int>> Insert(UserModel user)
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
		public async Task<Response<bool>> Update(UserModel user)
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
		public async Task<Response<bool>> ResetPassword(UserViewModel user)
		{
			var response = new Response<bool>();

			try
			{
				await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_User_Password_Reset", new
				{
					user.PasswordHash,
					user.PasswordSalt,
					user.PasswordResetToken,
					user.LastLoginDateUtc,
					user.LastLogoutDateUtc,
					user.LastPasswordChangeUtc,
					user.ObjectId
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

		public async Task<UserModel> GetUserByUsername(string username)
		{
			var results = await _db.LoadDataUsingProcedure<UserModel, dynamic>(storedProcedure: "dbo.SP_Security_User_GetByUsername", new { Username = username });

			return results.FirstOrDefault();
		}

		public async Task<UserModel> GetUsername(string username)
		{
			var query = $"SELECT * FROM [dbo].[Security_User] WITH(NOLOCK) WHERE Username = @username";
			var result = await _db.LoadDataUsingQuery<UserModel, dynamic>(query, new { Username = username });
			return result.FirstOrDefault();
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
		public async Task<Response<UserModel>> ValidateUserPassword(UserViewModel userInfo)
		{
			var response = new Response<UserModel>();

			// Retrieve the user by their ID
			var user = await GetById(userInfo.UserId);

			// Check if the user exists
			if (user == null)
			{
				response.IsSuccess = false;
				response.Message = "User not found.";
				return response;
			}

			// Extract stored password salt and hash
			var storedPasswordSalt = user.PasswordSalt;
			var storedPasswordHash = user.PasswordHash;

			// Verify the provided password against the stored hash and salt
			var isVerified = await VerifyPassword(userInfo.PreviousPassword, storedPasswordHash, storedPasswordSalt);

			if (isVerified)
			{
				response.Result = user;
				response.IsSuccess = true;
			}
			else
			{
				response.IsSuccess = false;
				response.Message = "Password verification failed.";
			}

			return response;
		}

		public Task<bool> VerifyPassword(string providedPassword, string storedPasswordHash, string storedPasswordSalt)
		{
			// Hash the provided password with the stored salt
			var providedPasswordHash = GetHashPassword(providedPassword, storedPasswordSalt);

			// Compare the provided password hash with the stored password hash
			var isMatch = providedPasswordHash == storedPasswordHash;

			// Return a completed Task with the result
			return Task.FromResult(isMatch);
		}
		public async Task<Response<bool>> ChangePassword(UserViewModel userInfo, Claim? claim = null)
		{
			var response = new Response<bool>();

			try
			{
				if (string.IsNullOrWhiteSpace(userInfo.ObjectId))
				{
                    var user = await GetById(userInfo.UserId);
					if (user != null)
					{ 
						userInfo.ObjectId = user.ObjectId;
					}
                    // Check if the user exists
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "User not found.";
                        return response;
                    }
                }
                var salt = CreateRandomSalt();
                var hashedPassword = GetHashPassword(userInfo.Password, salt);
                userInfo.PasswordHash = hashedPassword;
                userInfo.PasswordSalt = salt;

                if (claim!=null && !string.IsNullOrWhiteSpace(claim.Value))
				{
					// Assume ResetPassword method returns a Task<Response<bool>>

					if (claim.Value == PermissionConstants.Security_UserPasswordChangeForAdmin)
					{
						var result = await ResetPassword(userInfo);

						if (result.IsSuccess)
						{
							response.IsSuccess = true;
						}
						else
						{
							response.IsSuccess = false;
							response.Message = "Failed to reset password.";
						}
					}
					else
					{
                        response.IsSuccess = false;
                        response.Message = "You dont have enough permission for changes employee password !";
                    }

                    
					return response;
                }

				var validateUser = await ValidateUserPassword(userInfo);

				if (validateUser.Result != null && validateUser.IsSuccess)
				{
					userInfo.ObjectId = validateUser.Result.ObjectId;

					// Assume ResetPassword method returns a Task<Response<bool>>
					var result = await ResetPassword(userInfo);

					if (result.IsSuccess)
					{
						response.IsSuccess = true;
					}
					else
					{
						response.IsSuccess = false;
						response.Message = "Failed to reset password.";
					}
				}
				else
				{
					response.IsSuccess = false;
					response.Message = "User validation failed.";
				}
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = "An error occurred while changing password.";
				// Log the exception for debugging and auditing
				// logger.LogError(ex, "An error occurred while changing password.");
			}

			return response;
		}

	}
}
