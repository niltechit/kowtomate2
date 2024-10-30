using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientCategoryServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IClientCategoryBaseRuleService
{
    Task<Response<int>> Insert(ClientCategoryBaseRuleModel clientCategoryBaseRule);
    Task<Response<int>> Update(ClientCategoryBaseRuleModel clientCategoryBaseRule);
    Task<Response<bool>> Delete(int Id);
    Task<ClientCategoryBaseRuleModel> GetById(int Id);
    Task<List<ClientCategoryBaseRuleModel>> GetAll();
    Task<Response<int>> ClientCategoryRuleInsert(ClientCategoryRuleModel clientCategoryRule);
    Task<List<ClientCategoryRuleModel>> GetCategoryRuleByCategorygBaseRuleId(int clientCategoryBaseRuleId);
    Task<ClientCategoryRuleModel> ClientCategoryRuleGetById(int Id);
    Task<List<ClientCategoryBaseRuleModel>> ClientCategoryRuleGetByCompanyId(int companyId);
    Task<Response<int>> ClientCategoryRuleUpdate(ClientCategoryRuleModel clientCategoryRule);
}

