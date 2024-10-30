using CutOutWiz.Core;
using CutOutWiz.Services.Models.IBRModels;

namespace CutOutWiz.Services.IbrApiServices
{
    public interface IIbrApiService
    {
        Task<Response<IbrLoginResponse>> Login(IbrLoginRequest ibrLoginRequest);
        Task<Response<string>> RequestForIbrProcess(string masterOrderId, byte[] downloadImageBytes, string ibrToken, string fileName,string model_base_url);
        Task<Response<IbrDefaultSettingsApiResponse>> GetIbrGeneralSetting(string ibrToken);
        Task<Response<IbrOrderMasterInfoResponse>> GetOrderMasterId(IbrDefaultSettingsApiResponse ibrDefaultSettingsApiResponse, string ibrToken);
    }
}