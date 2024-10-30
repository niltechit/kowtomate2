using CutOutWiz.Services.DbAccess;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Email
{
    public class OperationEmailService : IOperationEmailService
    {
        private readonly ISqlDataAccess _db;
        private readonly IOperationEmailService _operationEmailService;
        public OperationEmailService(ISqlDataAccess db)
        {
            _db = db;
        }
        public async Task<List<ContactModel>> GetUserListByCompanyIdAndPermissionName(int companyId, string permissionName)
        {
            return await _db.LoadDataUsingProcedure<ContactModel, dynamic>(storedProcedure: "dbo.[SP_Security_Contact_GetContactsByPermissionAndCompanyId]", new { companyId = companyId,Permission=permissionName });
        }
        // TODO : Md Zakir Hossain
        //private async Task SendMailToAllOperation(string callerType)
        //{
        //    var userList = await _operationEmailService.GetUserListByCompanyIdAndPermissionName(Convert.ToInt32(_configuration["CompanyId"]), PermissionContants.OrderNewOrderEmailNotifyForOPeration);
        //    foreach (var user in userList)
        //    {
        //        var detailUrl = $"{_configuration["AppMainUrl"]}/order/details/{order.ObjectId}";

        //        var ordervm = new ClientOrderViewModel
        //        {
        //            Contact = user,
        //            DetailUrl = detailUrl,
        //            CreatedByContactId = loginUser.ContactId,
        //            OrderNumber = order.OrderNumber,
        //        };

        //        ordervm.MailType = callerType;
        //        await _workflowEmailService.SendOrderAddUpdateDeleteNotificationForCompanyOperationsTeam(ordervm);
        //    }
        //}
    }
}
