using CutOutWiz.Data;
using CutOutWiz.Data.Security;
using CutOutWiz.Services.Common;
using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.Security
{
    public class CustomAuthenticationService : ICustomAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;

        public CustomAuthenticationService(IUserService userService,
            ICompanyService companyService)
        {
            _userService = userService;
            _companyService = companyService;
        }

        public async Task<Response<User>> AuthenticateUser(LoginViewModel loginModel)
        {
            var response = new Response<User>();

            var user = await _userService.GetUserByUsername(loginModel.Username);

            if (user == null)
            {
                response.Message = "You username and/or password do not match. Pleaes try again.";
                return response;
            }

            //var st = _userService.GetHashPassword(loginModel.Password, user.PasswordSalt);

            if (user.PasswordHash != _userService.GetHashPassword(loginModel.Password, user.PasswordSalt))
            {
                response.Message = "You username and/or password do not match. Pleaes try again.";
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
    }
}
