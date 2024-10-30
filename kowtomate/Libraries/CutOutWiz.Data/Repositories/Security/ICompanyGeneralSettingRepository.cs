using CutOutWiz.Core;
using CutOutWiz.Data.Entities.Security;

namespace CutOutWiz.Data.Repositories.Security
{
    public interface ICompanyGeneralSettingRepository
	{
        Task<CompanyGeneralSettingEntity> GetGeneralSettingById(int Id);
        Task<Response<bool>> Delete(int Id);
        Task<CompanyGeneralSettingEntity> GetAllGeneralSettingsByCompanyId(int companyId);
        Task<Response<int>> Insert(CompanyGeneralSettingEntity generalSetting);
        Task<Response<bool>> Update(CompanyGeneralSettingEntity generalSetting);
        Task<List<CompanyGeneralSettingEntity>> GetAllCompanyGeneralSettingsByQuery(string query);
        Task<CompanyGeneralSettingEntity> GetGeneralSettingByCompanyId(int companyId);
	}
}
