using CutOutWiz.Core;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.HR;
using CutOutWiz.Services.Models.Security;
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
        Task<List<DesignationModel>> GetAll();
        Task<DesignationModel> GetById(int companyId);
        Task<DesignationModel> GetByObjectId(string objectId);
        Task<Core.Response<int>> Insert(DesignationModel company);
        Task<Core.Response<bool>> Update(DesignationModel company);
    }
}
