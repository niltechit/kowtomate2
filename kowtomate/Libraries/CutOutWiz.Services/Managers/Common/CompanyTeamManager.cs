
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.MapperHelper;
using CutOutWiz.Data.Repositories.Common;
using CutOutWiz.Data.Models.Common;
using CutOutWiz.Data.Entities.Common;
using CutOutWiz.Core;
using CutOutWiz.Data.DTOs.Common;

namespace CutOutWiz.Services.Managers.Common
{
    public class CompanyTeamManager : ICompanyTeamManager
    {
        private readonly IMapperHelperService _mapperHelperService;
        private readonly ICompanyTeamRepository _companyTeamRepository;
        public CompanyTeamManager(ICompanyTeamRepository companyTeamRepository,
            IMapperHelperService mapperHelperService)
        {
            _companyTeamRepository = companyTeamRepository;
            _mapperHelperService = mapperHelperService;
        }

        /// <summary>
        /// Get All Companys
        /// </summary>
        /// <returns></returns>
        public async Task<List<CompanyModel>> GetAll()
        {
            var entities = await _companyTeamRepository.GetAll();
            return await _mapperHelperService.MapToListAsync<CompanyEntity, CompanyModel>(entities);
        }

        /// <summary>
        /// Get company by company Id
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public async Task<CompanyModel> GetById(int companyId)
        {
            var entity = await _companyTeamRepository.GetById(companyId);
            return await _mapperHelperService.MapToSingleAsync<CompanyEntity, CompanyModel>(entity);
        }

        public async Task<CompanyTeamModel> GetTeamByCompanyId(int companyId)
        {
            var entity = await _companyTeamRepository.GetTeamByCompanyId(companyId);
            return await _mapperHelperService.MapToSingleAsync<CompanyTeamEntity, CompanyTeamModel>(entity);
        }

        public async Task<List<CompanyTeamModel>> GetByCompanyId(int companyId)
        {
            var entities = await _companyTeamRepository.GetByCompanyId(companyId);
            return await _mapperHelperService.MapToListAsync<CompanyTeamEntity, CompanyTeamModel>(entities);
        }

        public async Task<List<CompanyTeamModel>> GetCompanyTeamByCompanyId(int companyId)
        {
            var entities = await _companyTeamRepository.GetCompanyTeamByCompanyId(companyId);
            return await _mapperHelperService.MapToListAsync<CompanyTeamEntity, CompanyTeamModel>(entities);
        }

        //     /// <summary>
        //     /// Get by Object Id
        //     /// </summary>
        //     /// <param name="CompanyId"></param>
        //     /// <returns></returns>
        //     public async Task<CompanyModel> GetByObjectId(string objectId)
        //     {
        //var entity = await _companyTeamRepository.GetByObjectId(objectId);
        //return await _mapperHelperService.MapToSingleAsync<CompanyEntity, CompanyModel>(entity);
        //     }

        /// <summary>
        /// Insert company
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(List<CompanyTeamModel> companyTeams)
        {
            //TODO: Need  Refacotor in Business Layer and Data Layer
            //var count = 0 ;
            //var response = new Core.Response<int>();
            //try
            //{
            //    foreach (var companyTeam in companyTeams)
            //    {
            //        if (count <= 0)
            //        {
            //            await Delete(companyTeam.CompanyId);
            //        }
            //        count++;
            //        var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Common_CompanyTeam_Insert", new
            //        {
            //            companyTeam.CompanyId,
            //            companyTeam.TeamId,
            //            companyTeam.Status,
            //            companyTeam.CreatedByContactId,
            //            companyTeam.ObjectId

            //        });
            //    }
            //    //companyTeam.Id = newId;
            //    //response.Result = newId;
            //    response.IsSuccess = true;
            //    response.Message = StandardDataAccessMessages.SuccessMessaage;

            //}
            //catch (Exception ex)
            //{
            //    response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            //}
            //TODO: Add null check and other validation
            var companyTeamEntities = await _mapperHelperService.MapToListAsync<CompanyTeamModel, CompanyTeamEntity>(companyTeams);
            return await _companyTeamRepository.Insert(companyTeamEntities);
        }

        /// <summary>
        /// Update Company
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(CompanyTeamModel companyTeamModel)
        {
            //TODO: Add null check and other validation
            var companyTeamEntity = await _mapperHelperService.MapToSingleAsync<CompanyTeamModel, CompanyTeamEntity>(companyTeamModel);
            return await _companyTeamRepository.Update(companyTeamEntity);
        }

        /// <summary>
        /// Delete Company by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(int companyId)
        {
            return await _companyTeamRepository.Delete(companyId);
        }
        /// <summary>
        /// Adding New Company for Signup page
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<int>> SignupInsertCompany(SignUpViewModel signUpViewModel)
        {
            //TODO: Add null check and other validation
            var signUpDto = await _mapperHelperService.MapToSingleAsync<SignUpViewModel, SignUpDto>(signUpViewModel);
            return await _companyTeamRepository.SignupInsertCompany(signUpDto);
        }
    }
}
