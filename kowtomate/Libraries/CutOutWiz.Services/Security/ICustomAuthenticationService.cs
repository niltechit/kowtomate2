using CutOutWiz.Core;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Security
{
    public interface ICustomAuthenticationService
    {
        Task<Response<UserModel>> AuthenticateUser(LoginViewModel loginModel);
		/// <summary>
		/// When authenticate user then Return User And Company Information
		/// </summary>
		/// <param name="loginModel"></param>
		/// <returns> User And Company</returns>
		Task<(UserModel,CompanyModel)> AuthenticationUser(LoginViewModel loginModel);
    }
}