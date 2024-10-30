using CutOutWiz.Core.Utilities;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Services.BLL;
using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.ErrorLogServices
{
    public class ErrorLogService:IErrorLogService
    {
        #region Constructor

        public IActivityAppLogService _activityAppLogService;
        public ErrorLogService(IActivityAppLogService activityAppLogService)
        {
            _activityAppLogService = activityAppLogService;
        }

        #endregion

        #region Public Method
        public async Task LogFtpProcessingError(Exception ex, string methodName, byte category)
        {
            var activity = CreateActivityLog(ex, methodName, category);
            await _activityAppLogService.InsertAppErrorActivityLog(activity);
        }

        public async Task LogGeneralError(Exception ex, string methodName, byte category)
        {
            var activity = CreateActivityLog(ex, methodName, category);
            await _activityAppLogService.InsertAppErrorActivityLog(activity);
        }
        #endregion

        #region Private Method
        private CommonActivityLogViewModel CreateActivityLog(Exception ex, string methodName, byte category)
        {
            var loginUser = new LoginUserInfoViewModel
            {
                ContactId = AutomatedAppConstant.ContactId
            };

            return new CommonActivityLogViewModel
            {
                ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
                loginUser = loginUser,
                ErrorMessage = ex.Message,
                MethodName = methodName,
                RazorPage = "FtpOrderProcessService",
                Category = category
            };
        }

        #endregion
    }
}
