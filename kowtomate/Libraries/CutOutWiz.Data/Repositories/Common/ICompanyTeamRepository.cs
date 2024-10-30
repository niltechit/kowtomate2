using CutOutWiz.Core;
using CutOutWiz.Data.DTOs.Common;
using CutOutWiz.Data.Entities.Common;
using CutOutWiz.Data.Models.Common;

namespace CutOutWiz.Data.Repositories.Common
{
    public interface ICompanyTeamRepository
	{
        Task<Response<bool>> Delete(int companyId);
        Task<List<CompanyEntity>> GetAll();
        Task<CompanyEntity> GetById(int companyId);
        Task<CompanyTeamEntity> GetTeamByCompanyId(int companyId);
        Task<List<CompanyTeamEntity>> GetByCompanyId(int companyId);
        Task<List<CompanyTeamEntity>> GetCompanyTeamByCompanyId(int companyId);
        Task<Response<int>> Insert(List<CompanyTeamEntity> companyTeams);
        Task<Response<int>> SignupInsertCompany(SignUpDto model);
        Task<Response<bool>> Update(CompanyTeamEntity company);
    }
}