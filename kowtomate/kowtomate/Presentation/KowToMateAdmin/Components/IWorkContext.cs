using KowToMateAdmin.Models.Security;

namespace KowToMateAdmin
{
    public interface IWorkContext
    {
        LoginUserInfoViewModel LoginUserInfo { get; }
        public string AdminBaseUrl { get; }
    }
}