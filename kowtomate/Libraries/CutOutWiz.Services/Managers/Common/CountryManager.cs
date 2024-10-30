using CutOutWiz.Core;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.MapperHelper;
using CutOutWiz.Data.Repositories.Common;
using CutOutWiz.Data.Entities.Common;

namespace CutOutWiz.Services.Managers.Common
{
    public class CountryManager : ICountryManager
    {
        private readonly IMapperHelperService _mapperHelperService;
        private readonly ICountryRepository _countryRepository;

        public CountryManager(ICountryRepository countryRepository,
            IMapperHelperService mapperHelperService)
        {
            _countryRepository = countryRepository;
            _mapperHelperService = mapperHelperService;
        }

        /// <summary>
        /// Get All Countrys
        /// </summary>
        /// <returns></returns>
        public async Task<List<CountryModel>> GetAll()
        {
            var entities = await _countryRepository.GetAll();
            return await _mapperHelperService.MapToListAsync<CountryEntity, CountryModel>(entities);
        }

        /// <summary>
        /// Get country by country Id
        /// </summary>
        /// <param name="CountryId"></param>
        /// <returns></returns>
        public async Task<CountryModel> GetById(int countryId)
        {
            var entity = await _countryRepository.GetById(countryId);
            return await _mapperHelperService.MapToSingleAsync<CountryEntity, CountryModel>(entity);
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="CountryId"></param>
        /// <returns></returns>
        public async Task<CountryModel> GetByObjectId(string objectId)
        {
            var entity = await _countryRepository.GetByObjectId(objectId);
            return await _mapperHelperService.MapToSingleAsync<CountryEntity, CountryModel>(entity);
        }

        /// <summary>
        /// Insert country
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(CountryModel countryModel)
        {
            //Add validation logic here
            var entity = await _mapperHelperService.MapToSingleAsync<CountryModel, CountryEntity>(countryModel);
            return await _countryRepository.Insert(entity);
        }

        /// <summary>
        /// Update Country
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(CountryModel countryModel)
        {
            //Add validation logic here
            var entity = await _mapperHelperService.MapToSingleAsync<CountryModel, CountryEntity>(countryModel);
            return await _countryRepository.Update(entity);
        }

        /// <summary>
        /// Delete Country by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            return await _countryRepository.Delete(objectId);
        }

    }
}
