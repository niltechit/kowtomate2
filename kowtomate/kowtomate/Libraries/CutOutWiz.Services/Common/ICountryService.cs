using CutOutWiz.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Common
{
    public interface ICountryService
    {
        Task<Data.Response<bool>> Delete(string objectId);
        Task<List<Country>> GetAll();
        Task<Country> GetById(int countryId);
        Task<Country> GetByObjectId(string objectId);
        Task<Data.Response<int>> Insert(Country country);
        Task<Data.Response<bool>> Update(Country country);

    }
}
