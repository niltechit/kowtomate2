using CutOutWiz.Data;
using CutOutWiz.Data.Security;

namespace CutOutWiz.Services.Security
{
    public interface ICustomAuthenticationService
    {
        Task<Response<User>> AuthenticateUser(LoginViewModel loginModel);
    }
}