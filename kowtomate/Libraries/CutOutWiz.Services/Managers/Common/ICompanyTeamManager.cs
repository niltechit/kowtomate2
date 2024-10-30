using CutOutWiz.Core;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Managers.Common
{
    public interface ICompanyTeamManager
    {
        Task<Response<bool>> Delete(int companyId);
        Task<List<CompanyModel>> GetAll();
        Task<CompanyModel> GetById(int companyId);
        Task<CompanyTeamModel> GetTeamByCompanyId(int companyId);
        Task<List<CompanyTeamModel>> GetByCompanyId(int companyId);
        Task<List<CompanyTeamModel>> GetCompanyTeamByCompanyId(int companyId);
        Task<Response<int>> Insert(List<CompanyTeamModel> companyTeams);
        Task<Response<int>> SignupInsertCompany(SignUpViewModel model);
        Task<Response<bool>> Update(CompanyTeamModel company);
    }
}