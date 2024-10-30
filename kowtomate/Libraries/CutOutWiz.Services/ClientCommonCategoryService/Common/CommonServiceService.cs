using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientCategoryServices;
using CutOutWiz.Services;
using CutOutWiz.Services.DbAccess;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Data;

public class CommonServiceService : ICommonServiceService
{
    private readonly ISqlDataAccess _db;

    public CommonServiceService(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<Response<bool>> Delete(int Id)
    {
        var response = new Response<bool>();
        try
        {
            await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Common_Service_DeleteById", new
            {
                Id = Id,
                IsActive = 0,
                IsDeleted = 1,
            });

            response.IsSuccess = true;
            response.Message = StandardDataAccessMessages.SuccessMessaage;

        }
        catch (Exception ex)
        {
            response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
        }

        return response;
    }

    public async Task<List<CutOutWiz.Services.Models.ClientCategoryServices.CommonServiceModel>> GetAll()
    {
        return  await _db.LoadDataUsingProcedure<CutOutWiz.Services.Models.ClientCategoryServices.CommonServiceModel, dynamic>(storedProcedure: "dbo.SP_Common_Service_GetAll", new { });
    }

    public async Task<CutOutWiz.Services.Models.ClientCategoryServices.CommonServiceModel> GetById(int Id)
    {
        var result = await _db.LoadDataUsingProcedure<CutOutWiz.Services.Models.ClientCategoryServices.CommonServiceModel, dynamic>(storedProcedure: "dbo.SP_Common_Service_GetById", new { Id = Id });
        return result.FirstOrDefault();
    }

    public async Task<Response<int>> Insert(CutOutWiz.Services.Models.ClientCategoryServices.CommonServiceModel common)
    {
        var response = new Response<int>();
        common.CreatedDate = DateTime.Now;
        common.UpdatedDate = DateTime.Now;
        try
        {
            var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Common_Service_Insert", new
            {
                common.Name,
                common.TimeInMinutes,
                common.PriceInUSD,
                common.IsActive,
                common.CreatedDate,
                common.CreatedByUsername,
                common.UpdatedDate,
                common.UpdatedByUsername,
            });

            common.Id = newId;
            response.Result = newId;
            response.IsSuccess = true;
            response.Message = StandardDataAccessMessages.SuccessMessaage;

        }
        catch (Exception ex)
        {
            response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
        }

        return response;
    }

    public async Task<Response<bool>> Update(CutOutWiz.Services.Models.ClientCategoryServices.CommonServiceModel common)
    {
        var response = new Response<bool>();
        try
        {
            await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Common_Service_Update", new
            {
                common.Id,
                common.Name,
                common.TimeInMinutes,
                common.PriceInUSD,
                common.IsActive,
                common.UpdatedDate,
                common.UpdatedByUsername,
                common.IsDeleted
            });

            response.IsSuccess = true;
            response.Message = StandardDataAccessMessages.SuccessMessaage;

        }
        catch (Exception ex)
        {
            response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
        }

        return response;
    }
}

