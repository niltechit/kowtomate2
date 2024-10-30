using CutOutWiz.Core;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.Security;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.Security
{
    public class CustomAuthenticationService : ICustomAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly ICompanyManager _companyService;

        public CustomAuthenticationService(IUserService userService,
            ICompanyManager companyService)
        {
            _userService = userService;
            _companyService = companyService;
        }

        public async Task<Response<UserModel>> AuthenticateUser(LoginViewModel loginModel)
        {
            var response = new Response<UserModel>();

            var user = await _userService.GetUserByUsername(loginModel.Username);

            if (user == null)
            {
                response.Message = "You username and/or password do not match. Please try again.";
                return response;
            }

            //var st = _userService.GetHashPassword(loginModel.Password, user.PasswordSalt);

            if (user.PasswordHash != _userService.GetHashPassword(loginModel.Password, user.PasswordSalt))
            {
                response.Message = "You username and/or password do not match. Please try again.";
                return response;
            }

            if (user.Status != (int)GeneralStatus.Active)
            {
                response.Message = "You account is currently disable. Please contact with administrator.";
                return response;
            }

            response.Result = user;

            var company = await _companyService.GetById(user.CompanyId);

            if (company == null || company.Status != (int)GeneralStatus.Active)
            {
                response.Message = "You company account is currently disable. Please contact with administrator.";
                return response;
            }

            //Check subscription
            //TODO: check subscription status and and redirect to payment page
            response.IsSuccess = true;
            return response;
        }
        /// <summary>
        /// When authenticate user then Return User And Company Information
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        public async Task<(UserModel?, CompanyModel?)> AuthenticationUser(LoginViewModel loginModel)
        {
            var user = await _userService.GetUserByUsername(loginModel.Username);
            if (user == null || user.PasswordHash != _userService.GetHashPassword(loginModel.Password, user.PasswordSalt))
            {
                return (null, null);
            }
            if (user.Status != (int)GeneralStatus.Active)
            {
                return (null, null);
            }
            var company = await _companyService.GetById(user.CompanyId);

            if (company != null && company.Status != (int)GeneralStatus.Active)
            {
                return (user, null);
            }

            return (user, company);
        }
    }
}
