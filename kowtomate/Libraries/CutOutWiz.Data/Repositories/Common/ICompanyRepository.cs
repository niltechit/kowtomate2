using CutOutWiz.Core;
using CutOutWiz.Data.DTOs.Common;
using CutOutWiz.Data.Models.Common;
using CutOutWiz.Data.Security;

namespace CutOutWiz.Data.Repositories.Common
{
    public interface ICompanyRepository
	{
        Task<Core.Response<bool>> Delete(string objectId);
        Task<List<CompanyEntity>> GetAll();
        Task<List<CompanyEntity>> GetAllClientCompany();
        Task<CompanyEntity> GetById(int companyId);
        Task<RoleEntity> GetRoleByCompanyObjectId(string companyObjectId);
        Task<CompanyEntity> GetByObjectId(string objectId);
        Task<Response<int>> Insert(CompanyEntity company);
        Task<Response<bool>> Update(CompanyEntity company);
        Task<Response<int>> SignupInsertCompany(SignUpDto model);
        Task<List<CompanyEntity>> GetCompaniesById(int companyId);
        Task<CompanyEntity> GetCompanyByCode(string Code);
        /// <summary>
        /// Get company by company Code
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        Task<CompanyEntity> GetByCompanyCode(string code);
        Task<CompanyEntity> GetByCompanyName(string Name);
        Task<CompanyEntity> GetByCompanyEmail(string Email);
        Task<List<CompanyEntity>> GetAllClientCompanyByQuery(string query);
    }
}
