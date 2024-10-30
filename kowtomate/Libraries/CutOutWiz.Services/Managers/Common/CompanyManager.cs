using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Data.Repositories.Common;
using CutOutWiz.Services.MapperHelper;
using CutOutWiz.Data.Security;
using CutOutWiz.Data.Models.Common;
using CutOutWiz.Core;
using CutOutWiz.Data.DTOs.Common;

namespace CutOutWiz.Services.Managers.Common
{
    public class CompanyManager : ICompanyManager
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapperHelperService _mapperHelperService;

        public CompanyManager(ICompanyRepository companyRepository,
            IMapperHelperService mapperHelperService)
        {
            _companyRepository = companyRepository;
            _mapperHelperService = mapperHelperService;
        }

        /// <summary>
        /// Get All Companys
        /// </summary>
        /// <returns></returns>
        public async Task<List<CompanyModel>> GetAll()
        {
            var entities = await _companyRepository.GetAll();
            return await _mapperHelperService.MapToListAsync<CompanyEntity, CompanyModel>(entities);
        }

        /// <summary>
        /// Get All Companys
        /// </summary>
        /// <returns></returns>
        public async Task<List<CompanyModel>> GetAllClientCompany()
        {
            var entities = await _companyRepository.GetAllClientCompany();
            return await _mapperHelperService.MapToListAsync<CompanyEntity, CompanyModel>(entities);
        }

        /// <summary>
        /// Get company by company Id
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public async Task<CompanyModel> GetById(int companyId)
        {
            var entity = await _companyRepository.GetById(companyId);
            return await _mapperHelperService.MapToSingleAsync<CompanyEntity, CompanyModel>(entity);
        }

        /// <summary>
        /// Get company by company Code
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public async Task<CompanyModel> GetByCompanyCode(string code)
        {
            var entity = await _companyRepository.GetByCompanyCode(code);
            return await _mapperHelperService.MapToSingleAsync<CompanyEntity, CompanyModel>(entity);
        }

        public async Task<CompanyModel> GetByCompanyName(string name)
        {
            var entity = await _companyRepository.GetByCompanyName(name);
            return await _mapperHelperService.MapToSingleAsync<CompanyEntity, CompanyModel>(entity);
        }

        public async Task<CompanyModel> GetByCompanyEmail(string email)
        {
            var entity = await _companyRepository.GetByCompanyEmail(email);
            return await _mapperHelperService.MapToSingleAsync<CompanyEntity, CompanyModel>(entity);
        }

        public async Task<List<CompanyModel>> GetCompaniesById(int companyId)
        {
            var entities = await _companyRepository.GetCompaniesById(companyId);
            return await _mapperHelperService.MapToListAsync<CompanyEntity, CompanyModel>(entities);
        }

        public async Task<CompanyModel> GetCompanyByCode(string code)
        {
            var entity = await _companyRepository.GetCompanyByCode(code);
            return await _mapperHelperService.MapToSingleAsync<CompanyEntity, CompanyModel>(entity);
        }

        public async Task<RoleModel> GetRoleByCompanyObjectId(string companyObjectId)
        {
            var entity = await _companyRepository.GetRoleByCompanyObjectId(companyObjectId);
            return await _mapperHelperService.MapToSingleAsync<RoleEntity, RoleModel>(entity);
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public async Task<CompanyModel> GetByObjectId(string objectId)
        {
            var entity = await _companyRepository.GetByObjectId(objectId);
            return await _mapperHelperService.MapToSingleAsync<CompanyEntity, CompanyModel>(entity);
        }

        /// <summary>
        /// Insert company
        /// </summary>
        /// <param name="companyModel"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(CompanyModel companyModel)
        {
            //Add validation logic here
            var entity = await _mapperHelperService.MapToSingleAsync<CompanyModel, CompanyEntity>(companyModel);
            return await _companyRepository.Insert(entity);
        }

        /// <summary>
        /// Update Company
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(CompanyModel companyModel)
        {
            var entity = await _mapperHelperService.MapToSingleAsync<CompanyModel, CompanyEntity>(companyModel);
            return await _companyRepository.Update(entity);
        }

        /// <summary>
        /// Delete Company by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            return await _companyRepository.Delete(objectId);
        }

        /// <summary>
        /// Adding New Company for Signup page
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<int>> SignupInsertCompany(SignUpViewModel model)
        {
            var signUpDto = await _mapperHelperService.MapToSingleAsync<SignUpViewModel, SignUpDto>(model);
            return await _companyRepository.SignupInsertCompany(signUpDto);
        }

        public async Task<List<CompanyModel>> GetAllClientCompanyByQuery(string query)
        {
            var entities = await _companyRepository.GetAllClientCompanyByQuery(query);
            return await _mapperHelperService.MapToListAsync<CompanyEntity, CompanyModel>(entities);
        }
    }
}
