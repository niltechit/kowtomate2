using CutOutWiz.Core;
using CutOutWiz.Data.Entities.Common;

namespace CutOutWiz.Data.Repositories.Common
{
    public interface ICountryRepository
	{
        Task<Response<bool>> Delete(string objectId);
        Task<List<CountryEntity>> GetAll();
        Task<CountryEntity> GetById(int countryId);
        Task<CountryEntity> GetByObjectId(string objectId);
        Task<Response<int>> Insert(CountryEntity country);
        Task<Response<bool>> Update(CountryEntity country);
    }
}
