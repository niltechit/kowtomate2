using CutOutWiz.Core;
using CutOutWiz.Services.Models.FtpModels;

namespace CutOutWiz.Services.AutomationAppServices.FtpOrderProcess
{
    public interface IClientExternalOrderFTPSetupService
    {
        Task<List<ClientExternalOrderFTPSetupModel>> GetAllEnabledFtps();
        Task<Response<bool>> Delete(int companyId);
        Task<ClientExternalOrderFTPSetupModel> GetByClientCompanyId(int copanyId);
        Task<Response<int>> Insert(ClientExternalOrderFTPSetupModel fileServer);
        Task<Response<bool>> Update(ClientExternalOrderFTPSetupModel fileServer);
        Task<ClientExternalOrderFTPSetupModel> GetById(int id);
    }
}