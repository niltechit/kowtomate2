using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Logs;
using KowToMateAdmin.Models;
using KowToMateAdmin.Models.Security;
using static CutOutWiz.Core.Utilities.Enums;

namespace KowToMateAdmin.Services
{
	public class AcitivityLogCommonMethodService : IAcitivityLogCommonMethodService
	{
		private readonly ILogServices _activityLogService;
		public AcitivityLogCommonMethodService(ILogServices acitivityLogService)
		{
			_activityLogService= acitivityLogService;
		}
		public async Task InsertErrorActivityLog(int orderId, string methodName, string errorMessage, byte category, LoginUserInfoViewModel loginUser,string razorPage=null)
		{
			try
			{
				ActivityLogModel activityLog = new ActivityLogModel();
				activityLog.ActivityLogFor = ActivityLogForConstants.Order;
				activityLog.PrimaryId = orderId;
				activityLog.Description = $"Page: '{razorPage}', Method: '{methodName}', Error: {errorMessage} on {DateTime.Now}";
				activityLog.CreatedDate = DateTime.Now;
				activityLog.CreatedByContactId = loginUser.ContactId;
				activityLog.ObjectId = Guid.NewGuid().ToString();
				activityLog.CompanyObjectId = loginUser.CompanyObjectId;
				activityLog.ContactObjectId = loginUser.UserObjectId;
				activityLog.Category = category;
				activityLog.Type = (byte)ActivityLogType.Error;

				await _activityLogService.Insert(activityLog);
			}
			catch (Exception ex)
			{
				
			}

		}
        public async Task InsertErrorActivityLog(CommonActivityLogViewModel model)
        {
            try
            {
                ActivityLogModel activityLog = new ActivityLogModel();
				activityLog.ActivityLogFor = model.ActivityLogFor;
                activityLog.PrimaryId = model.PrimaryId;
                activityLog.Description = $"Page: '{model.RazorPage}', Method: '{model.MethodName}', Error: {model.ErrorMessage} on {DateTime.Now}";
                activityLog.CreatedDate = DateTime.Now;
                activityLog.CreatedByContactId = model.loginUser.ContactId;
                activityLog.ObjectId = Guid.NewGuid().ToString();
                activityLog.CompanyObjectId = model.loginUser.CompanyObjectId;
                activityLog.ContactObjectId = model.loginUser.UserObjectId;
                activityLog.Category = model.Category;
				if (model.Type > 0)
				{
                    activityLog.Type = model.Type;
                }
				else
				{
					activityLog.Type = (byte)ActivityLogType.Error;
				}
                await _activityLogService.Insert(activityLog);
            }
            catch (Exception ex)
            {

            }

        }
    }
}
