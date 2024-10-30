
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.FtpModels;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.AutomationAppServices.FtpOrderProcess
{
    public interface IAutoOrderDeliveryService
    {
        Task<Response<List<ClientOrderItemModel>>> MoveAsCompletedOrder(List<ClientOrderItemModel> clientOrderItems, ClientOrderModel order);

        Task<Response<List<ClientOrderItemModel>>> MoveOrderToClientFtp(List<ClientOrderItemModel> clientOrderItems, ClientOrderModel order, ClientExternalOrderFTPSetupModel tempClientOrderFtp, CompanyGeneralSettingModel companyGeneralSetting = null);
        Task<Response<List<ClientOrderItemModel>>> MoveOrderToClientSFtp(List<ClientOrderItemModel> clientOrderItems, ClientOrderModel order, ClientExternalOrderFTPSetupModel tempClientOrderFtp);
        Task<Response<List<ClientOrderItemModel>>> MoveOrderItemIdsAsCompletedOrder(List<long> clientOrderItems, ClientOrderModel order);
        Task<bool> MoveOrderAsZipToClientFtp(string sourcePath, ClientExternalOrderFTPSetupModel tempClientOrderFtp);
        Task<Response<bool>> SendHotkeyFileToFtp(string locatPath, string batchName, ClientExternalOrderFTPSetupModel tempClientOrderFtp);


    }
}
