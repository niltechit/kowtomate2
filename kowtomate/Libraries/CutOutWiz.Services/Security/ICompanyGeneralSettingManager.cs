using CutOutWiz.Core;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Security
{
    public interface ICompanyGeneralSettingManager
    {
        Task<CompanyGeneralSettingModel> GetGeneralSettingById(int Id);
        Task<Response<bool>> Delete(int Id);
        Task<CompanyGeneralSettingModel> GetAllGeneralSettingsByCompanyId(int companyId);
        Task<Response<int>> Insert(CompanyGeneralSettingModel generalSetting);
        Task<Response<bool>> Update(CompanyGeneralSettingModel generalSetting);
        Task<List<CompanyGeneralSettingModel>> GetAllCompanyGeneralSettingsByQuery(string query);
        Task<CompanyGeneralSettingModel> GetGeneralSettingByCompanyId(int companyId);
	}
}
