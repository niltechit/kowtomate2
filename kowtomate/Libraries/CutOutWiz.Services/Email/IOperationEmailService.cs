using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Email
{
    public interface IOperationEmailService
    {
        Task<List<ContactModel>> GetUserListByCompanyIdAndPermissionName(int companyId, string permissionName);
        //Task SendMailToAllOperation(string callerType);
    }
}
