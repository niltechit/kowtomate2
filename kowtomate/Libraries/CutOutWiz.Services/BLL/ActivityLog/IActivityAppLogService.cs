using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientCategoryServices;
using CutOutWiz.Core.Models.ViewModel;

namespace CutOutWiz.Services.BLL
{
	public interface IActivityAppLogService
	{
        Task AppStartLog(string consoleAppName);
        Task AppEndLog(string consoleAppName);

        Task InsertAppErrorActivityLog(CommonActivityLogViewModel model);
		Task InsertAppDownloadToEditorPcActivityLog(int clientOrderItem, string message,int contactId = AutomatedAppConstant.ContactId);
        Task<Response<int>> ClientCategoryChangeLogInsert(ClientCategoryChangeLogModel clientCategoryChangeLog);
    }
}
