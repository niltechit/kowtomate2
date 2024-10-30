using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientCategoryServices;


public interface ICommonServiceService
{
    Task<Response<int>> Insert(CutOutWiz.Services.Models.ClientCategoryServices.CommonServiceModel common);
    Task<Response<bool>> Update(CutOutWiz.Services.Models.ClientCategoryServices.CommonServiceModel common);
    Task<Response<bool>> Delete(int Id);
    Task<CutOutWiz.Services.Models.ClientCategoryServices.CommonServiceModel> GetById(int Id);
    Task<List<CutOutWiz.Services.Models.ClientCategoryServices.CommonServiceModel>> GetAll();
}

