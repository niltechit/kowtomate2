using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.UI;

namespace CutOutWiz.Services.UI
{
    public interface IGridViewSetupService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<GridViewFilterModel> GetById(int gridVewFilterId);
        Task<List<GridViewSetupColumnSlim>> GetColumnsByGridViewSetupId(int gridViewSetupId);
        Task<List<GridViewFilterModel>> GetGridViewFiltersBySetupId(int contactId, int gridViewSetupId);
        Task<List<GridViewSetupModel>> GetListByContactId(Enums.GridViewFor gridViewFor, int contactId);
        Task<Response<bool>> GridViewFilterDelete(int gridViewFilterId);
        Task<Response<int>> GridViewFilterInsert(GridViewFilterModel gridVewFilter);
        Task<Response<int>> GridViewFilterUpdate(GridViewFilterModel gridVewFilter);
        Task<Response<int>> Insert(GridViewSetupModel gridViewSetup);
        Task<Response<bool>> TemplateColumnsInsertOrUpdateBySetupId(int gridViewSetupId, List<GridViewSetupColumnSlim> columns);
        Task<Response<int>> Update(GridViewSetupModel gridViewSetup);

        /// <summary>
        /// Get List by dynamicReportInfoId and contact id
        /// </summary>
        /// <returns></returns>
        Task<List<GridViewSetupModel>> GetListByDynamicReportIdContactId(int dynamicReportInfoId, int contactId);
    }
}