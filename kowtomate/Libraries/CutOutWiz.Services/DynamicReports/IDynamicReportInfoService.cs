using CutOutWiz.Core;
using CutOutWiz.Services.Models.DynamicReports;

namespace CutOutWiz.Services.DynamicReports
{
    public interface IDynamicReportInfoService
    {
        #region Dynamic Report Info
        Task<Response<bool>> Delete(string objectId);
        Task<List<DynamicReportInfoModel>> GetAll();

        /// <summary>
        /// Get All Dynamic Report Info For Dropdownlist
        /// </summary>
        /// <returns></returns>
        Task<List<DynamicReportInfoSlim>> GetAllDynamicReportInfoForDropdownlist();

        /// <summary>
        /// Get All Dynamic Reports by RoleModel Object Id
        /// </summary>
        /// <returns></returns>
        Task<List<DynamicReportInfoModel>> GetOnlyAssignedReportsByRoleObjectId(string userObjectId);
        Task<DynamicReportInfoModel> GetById(int dynamicReportInfoId);
        Task<DynamicReportInfoModel> GetReportInfoByName(string reportName);
        Task<DynamicReportInfoModel> GetByObjectId(string objectId);
        Task<Response<int>> Insert(DynamicReportInfoModel dynamicReportInfo);
        Task<Response<bool>> Update(DynamicReportInfoModel dynamicReportInfo);
        Task<Response<bool>> CloneDynamicReportInfo(DynamicReportInfoModel model);
        #endregion

        #region Dynamic Table Column
        Task<Response<bool>> DeleteTableColumn(int dynamicReportInfoId, int tableColumn);
        Task<ReportTableColumnModel> GetTableColumnByTableColumnId(int dynamicReportInfoId);
        Task<ReportTableColumnModel> GetTableColumnByTableFieldName(string fieldName);
        Task<Response<int>> InsertTableColumn(ReportTableColumnModel dynamicReportInfo);
        Task<Response<bool>> UpdateTableColumn(ReportTableColumnModel dynamicReportInfo);
        Task<List<ReportTableColumnModel>> GetAllTableColumnByDynamicReportInfoId(int dynamicReportInfoId);
        #endregion

        #region Dynamic Report Info
        Task<List<ReportTableColumnModel>> GetTableColumnByDynamicReportInfoId(int dynamicReportInfoId);
        Task<IEnumerable<IDictionary<string, object>>> GetRecordsDirectoryByFilterWithPaging(DynamicReportFilter filter);
        Task<IEnumerable<IDictionary<string, object>>> GetRecordsDirectoryByFilterWithoutPaging(DynamicReportFilter filter);
        #endregion

        #region Manage Join 
        /// <summary>
        /// Get Join Info by DynamicReportInfoId
        /// </summary>
        /// <param name="dynamicReportInfoId"></param>
        /// <returns></returns>
        Task<List<ReportJoinInfoModel>> GetJoinInfosByDynamicReportInfoId(int dynamicReportInfoId);
        #endregion

        #region Report Join

        Task<List<ReportJoinInfoModel>> GetReportJoinInfoListByDynamicReportInfoId(int dynamicReportInfoId);

        Task<ReportJoinInfoModel> GetReportJoinInfoById(int id);

        Task<Response<int>> InsertReportJoinInfo(ReportJoinInfoModel model);

        Task<Response<bool>> UpdateReportJoinInfo(ReportJoinInfoModel model);

        Task<Response<bool>> DeleteReportJoinInfo(int id);

        #endregion

        #region Get Data using dynaic query
        // <summary>
        /// Get Directory Using Dynamic Query By Filter With Paging
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<IDictionary<string, object>>> GetDirectoryUsingDynamicQueryByFilterWithPaging(DynamicReportFilter filter);

        /// <summary>
        /// Get Directory Using Dynamic Query By Filter Without Paging
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<IDictionary<string, object>>> GetDirectoryUsingDynamicQueryByFilterWithoutPaging(DynamicReportFilter filter);
        #endregion

    }
}
