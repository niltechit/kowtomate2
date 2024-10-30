using CutOutWiz.Data.Common;
using CutOutWiz.Data.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Common
{
    public interface ICompanyService
    {
        Task<Data.Response<bool>> Delete(string objectId);
        Task<List<Company>> GetAll();
        Task<Company> GetById(int companyId);
        Task<Company> GetByObjectId(string objectId);
        Task<Data.Response<int>> Insert(Company company);
        Task<Data.Response<bool>> Update(Company company);
    }
}
