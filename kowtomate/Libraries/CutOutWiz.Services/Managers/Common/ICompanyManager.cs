using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Managers.Common
{
    public interface ICompanyManager
    {
        Task<Core.Response<bool>> Delete(string objectId);
        Task<List<CompanyModel>> GetAll();
        Task<List<CompanyModel>> GetAllClientCompany();
        Task<CompanyModel> GetById(int companyId);
        Task<RoleModel> GetRoleByCompanyObjectId(string companyObjectId);
        Task<CompanyModel> GetByObjectId(string objectId);
        Task<Core.Response<int>> Insert(CompanyModel company);
        Task<Core.Response<bool>> Update(CompanyModel company);
        Task<Core.Response<int>> SignupInsertCompany(SignUpViewModel model);
        Task<List<CompanyModel>> GetCompaniesById(int companyId);
        Task<CompanyModel> GetCompanyByCode(string Code);
        /// <summary>
        /// Get company by company Code
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        Task<CompanyModel> GetByCompanyCode(string code);
        Task<CompanyModel> GetByCompanyName(string Name);
        Task<CompanyModel> GetByCompanyEmail(string Email);
        Task<List<CompanyModel>> GetAllClientCompanyByQuery(string query);
    }
}
