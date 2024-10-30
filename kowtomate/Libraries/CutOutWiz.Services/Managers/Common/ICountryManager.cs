using CutOutWiz.Core;
using CutOutWiz.Services.Models.Common;

namespace CutOutWiz.Services.Managers.Common
{
    public interface ICountryManager
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<CountryModel>> GetAll();
        Task<CountryModel> GetById(int countryId);
        Task<CountryModel> GetByObjectId(string objectId);
        Task<Response<int>> Insert(CountryModel country);
        Task<Response<bool>> Update(CountryModel country);
    }
}
