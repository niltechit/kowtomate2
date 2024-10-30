using CutOutWiz.Core;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.MapperHelper;
using CutOutWiz.Data.Entities.Security;
using CutOutWiz.Data.Repositories.Security;


namespace CutOutWiz.Services.Security
{
    public class CompanyGeneralSettingManager : ICompanyGeneralSettingManager
    {
		private readonly IMapperHelperService _mapperHelperService;
		private readonly ICompanyGeneralSettingRepository _companyGeneralSettingRepository;

		public CompanyGeneralSettingManager(ICompanyGeneralSettingRepository companyGeneralSettingRepository,
			IMapperHelperService mapperHelperService)
		{
			_companyGeneralSettingRepository = companyGeneralSettingRepository;
			_mapperHelperService = mapperHelperService;
		}

        public async Task<Response<bool>> Delete(int Id)
        {
            //TODO: Check Id id greater then 0 later 
            return await _companyGeneralSettingRepository.Delete(Id);
        }
        
        public async Task<CompanyGeneralSettingModel> GetGeneralSettingById(int id)
        {
			var entity = await _companyGeneralSettingRepository.GetGeneralSettingById(id);
			return await _mapperHelperService.MapToSingleAsync<CompanyGeneralSettingEntity, CompanyGeneralSettingModel>(entity);
        }

        /// <summary>
        /// Get General Settings by Company Id
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
		public async Task<CompanyGeneralSettingModel> GetGeneralSettingByCompanyId(int companyId)
		{
			var entity = await _companyGeneralSettingRepository.GetGeneralSettingByCompanyId(companyId);
			return await _mapperHelperService.MapToSingleAsync<CompanyGeneralSettingEntity, CompanyGeneralSettingModel>(entity);
		}

		/// <summary>
		/// Get General Settings by Company Id similar to GetGeneralSettingByCompanyId TODO: remove one of them
		/// </summary>
		/// <param name="companyId"></param>
		/// <returns></returns>
		public async Task<CompanyGeneralSettingModel> GetAllGeneralSettingsByCompanyId(int companyId)
        {
			var entity = await _companyGeneralSettingRepository.GetAllGeneralSettingsByCompanyId(companyId);
			return await _mapperHelperService.MapToSingleAsync<CompanyGeneralSettingEntity, CompanyGeneralSettingModel>(entity);
        }

        public async Task<Response<int>> Insert(CompanyGeneralSettingModel companyGeneralSettingModel)
        {
            var response = new Response<int>();
            //Validate data
            if (companyGeneralSettingModel == null)
            {
                response.Message = "CompanyGeneralSetting should not be empty.";
                return response;
			}

			var entity = await _mapperHelperService.MapToSingleAsync<CompanyGeneralSettingModel, CompanyGeneralSettingEntity>(companyGeneralSettingModel);
			return await _companyGeneralSettingRepository.Insert(entity);
        }

        public async Task<Response<bool>> Update(CompanyGeneralSettingModel companyGeneralSettingModel)
        {
			var response = new Response<bool>();
			//Validate data
			if (companyGeneralSettingModel == null)
			{
				response.Message = "CompanyGeneralSetting should not be empty.";
				return response;
			}

			var entity = await _mapperHelperService.MapToSingleAsync<CompanyGeneralSettingModel, CompanyGeneralSettingEntity>(companyGeneralSettingModel);
			return await _companyGeneralSettingRepository.Update(entity);
		}

		/// <summary>
		/// TODO: Review this method and move sql to Data Layer
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
        public async Task<List<CompanyGeneralSettingModel>> GetAllCompanyGeneralSettingsByQuery(string query)
        {
            var entities = await _companyGeneralSettingRepository.GetAllCompanyGeneralSettingsByQuery(query);
			return await _mapperHelperService.MapToListAsync<CompanyGeneralSettingEntity, CompanyGeneralSettingModel>(entities);
		}
    }
}
