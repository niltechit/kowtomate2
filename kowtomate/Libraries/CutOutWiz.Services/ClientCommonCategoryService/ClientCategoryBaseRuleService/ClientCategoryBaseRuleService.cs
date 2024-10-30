using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientCategoryServices;
using CutOutWiz.Services;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Data;

public class ClientCategoryBaseRuleService : IClientCategoryBaseRuleService
{
    private readonly ISqlDataAccess _db;

    public ClientCategoryBaseRuleService(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<Response<bool>> Delete(int Id)
    {
        var response = new Response<bool>();
        try
        {
            await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_ClientCategoryBaseRule_DeleteById", new
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

    public async Task<List<ClientCategoryBaseRuleModel>> GetAll()
    {
        var result = await _db.LoadDataUsingProcedure<ClientCategoryBaseRuleModel, dynamic>(storedProcedure: "dbo.SP_ClientCategoryBaseRule_GetAll", new { });
        return result.ToList();
    }

    public async Task<ClientCategoryBaseRuleModel> GetById(int Id)
    {
        var result = await _db.LoadDataUsingProcedure<ClientCategoryBaseRuleModel, dynamic>(storedProcedure: "dbo.SP_ClientCategoryBaseRule_GetAllById", new {id=Id });
        return result.FirstOrDefault();
    }

    public async Task<List<ClientCategoryBaseRuleModel>> ClientCategoryRuleGetByCompanyId(int companyId)
    {
        var result = await _db.LoadDataUsingProcedure<ClientCategoryBaseRuleModel, dynamic>(storedProcedure: "dbo.SP_ClientCategoryBaseRule_GetAllByCompanyId", new { companyId = companyId });
        return result.ToList();
    }

    public async Task<Response<int>> Insert(ClientCategoryBaseRuleModel clientCategoryBaseRule)
    {
        var response = new Response<int>();

        clientCategoryBaseRule.CreatedDate = DateTime.Now;

        try
        {
            var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_ClientCategoryBaseRule_Insert", new
            {
                clientCategoryBaseRule.Name,
                clientCategoryBaseRule.CompanyId,
                clientCategoryBaseRule.ClientCategoryId,
                clientCategoryBaseRule.IsActive,
                clientCategoryBaseRule.CreatedDate,
                clientCategoryBaseRule.CreatedByContactId,
                
            });

            clientCategoryBaseRule.Id = newId;
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

    public async Task<Response<int>> Update(ClientCategoryBaseRuleModel clientCategoryBaseRule)
    {
        var response = new Response<int>();
        clientCategoryBaseRule.UpdatedDate = DateTime.Now;
        try
        {
            await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_ClientCategoryBaseRule_Update", new
            {
                clientCategoryBaseRule.Id,
                clientCategoryBaseRule.CompanyId,
                clientCategoryBaseRule.Name,
                clientCategoryBaseRule.ClientCategoryId,
                clientCategoryBaseRule.IsActive,
                clientCategoryBaseRule.UpdatedDate,
                clientCategoryBaseRule.UpdatedByContactId,
                clientCategoryBaseRule.IsDeleted,
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

    #region ClientCategoryRule
    public async Task<Response<int>> ClientCategoryRuleInsert(ClientCategoryRuleModel clientCategoryRule)
    {
        var response = new Response<int>();

        clientCategoryRule.CreatedDate = DateTime.Now;

        try
        {
            var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_ClientCategoryRule_Insert", new
            {
                clientCategoryRule.Name,
                clientCategoryRule.ClientCategoryBaseRuleId,
                clientCategoryRule.Label,
                clientCategoryRule.Indicator,
                clientCategoryRule.FilterType,
                clientCategoryRule.ExecutionOrder,
                clientCategoryRule.IsActive,
                clientCategoryRule.CreatedByContactId,
                clientCategoryRule.CreatedDate,
            });

            clientCategoryRule.Id = newId;
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

    public async Task<ClientCategoryRuleModel> ClientCategoryRuleGetById(int Id)
    {
        var result = await _db.LoadDataUsingProcedure<ClientCategoryRuleModel, dynamic>(storedProcedure: "dbo.SP_ClientCategoryRule_GetAllById", new { id = Id });
        return result.FirstOrDefault();
    }
    
    public async Task<List<ClientCategoryRuleModel>> GetCategoryRuleByCategorygBaseRuleId(int clientCategoryBaseRuleId)
    {
        var result = await _db.LoadDataUsingProcedure<ClientCategoryRuleModel, dynamic>(storedProcedure: "dbo.SP_ClientCategoryRule_GetAllByCategorygBaseRuleId", new { clientCategoryBaseRuleId = clientCategoryBaseRuleId });
        return result;
    }

    public async Task<Response<int>> ClientCategoryRuleUpdate(ClientCategoryRuleModel clientCategoryRule)
    {
        var response = new Response<int>();
        clientCategoryRule.UpdatedDate = DateTime.Now;
        try
        {
            await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_ClientCategoryRule_Update", new
            {
                clientCategoryRule.Id,
                clientCategoryRule.Name,
                clientCategoryRule.Label,
                clientCategoryRule.Indicator,
                clientCategoryRule.FilterType,
                clientCategoryRule.ExecutionOrder,
                clientCategoryRule.IsActive,
                clientCategoryRule.UpdatedDate,
                clientCategoryRule.UpdatedByContactId,
                clientCategoryRule.IsDeleted,
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
    #endregion
}

