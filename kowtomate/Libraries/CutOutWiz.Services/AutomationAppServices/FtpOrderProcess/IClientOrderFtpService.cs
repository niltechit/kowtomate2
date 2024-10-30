using CutOutWiz.Core;
using CutOutWiz.Services.Models.FtpModels;

namespace CutOutWiz.Services.AutomationAppServices.FtpOrderProcess
{
    public interface IClientOrderFtpService
    {
        Task<List<ClientOrderFtpModel>> GetAllEnabledClientOrderFtps();
        Task<Response<bool>> Delete(int companyId);
        Task<ClientOrderFtpModel> GetByClientOrderFtpsCompanyId(int Id);
        Task<Response<int>> Insert(ClientOrderFtpModel fileServer);
        Task<Response<bool>> Update(ClientOrderFtpModel fileServer);
        Task<Response<bool>> UpdateIsEnableTrueForLocalFtp(int ClientCompanyId);
        Task<Response<bool>> UpdateIsEnableFalseForLocalFtp(int ClientCompanyId);
        Task<Response<List<ClientOrderFtpModel>>> GetAllInternalFtp(int? companyId = null);
        Task<List<ClientOrderFtpModel>> GetAllClientOrderFtps();
        Task<List<ClientOrderFtpModel>> GetClientOrderFtpsListByCompanyId(int companyId);
        Task<List<ClientOrderFtpModel>> GetClientDestinationFtpsCompanyId(int companyId);

    }
}