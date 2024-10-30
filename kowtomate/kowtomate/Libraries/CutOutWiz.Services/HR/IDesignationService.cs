using CutOutWiz.Data;
using CutOutWiz.Data.Common;
using CutOutWiz.Data.HR;
using CutOutWiz.Data.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.HR
{
    public interface IDesignationService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<Designation>> GetAll();
        Task<Designation> GetById(int companyId);
        Task<Designation> GetByObjectId(string objectId);
        Task<Data.Response<int>> Insert(Designation company);
        Task<Data.Response<bool>> Update(Designation company);
    }
}
