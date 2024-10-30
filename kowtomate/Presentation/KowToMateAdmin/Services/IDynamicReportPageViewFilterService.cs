using CutOutWiz.Services.Models.DynamicReports;
using CutOutWiz.Services.Models.UI;
using KowToMateAdmin.Models.DynamicFilter;
using Radzen;
using Radzen.Blazor;
using System.Text.RegularExpressions;
using static CutOutWiz.Core.Utilities.Enums;

namespace KowToMateAdmin.Services
{
    public interface IDynamicReportPageViewFilterService
    {
        /// <summary>
        /// Populate filter values
        /// </summary>
        /// <param name="listOfFilter"></param>
        /// <param name="allTableColumns"></param>
        void PopulateSetupFitlerValues(List<CompositeFilterDescriptor> listOfFilter, List<ReportTableColumnModel> allTableColumns);

        Task<List<GridViewSetupModel>> LoadGridViewTemplates(int? selectedDynamicReportInfoId, DynamicReportFilter dynamicReportFilter,
            DynamicReportPagePropertyVM dynamicReportPagePropertyVM, int loginUserContactId
            );

        Task LoadGridViewFilters(int loginUserContactId, DynamicReportFilter dynamicReportFilter,
            DynamicReportPagePropertyVM dynamicReportPagePropertyVM, bool relaodGrid = true, int newSelectedGridViewFilterId = 0);

        Task<IEnumerable<IDictionary<string, object>>> PopulateFilterColumnsAndQueryAndLoadData(LoadDataArgs args,
            DynamicReportFilter dynamicReportFilter, RadzenDataFilter<IDictionary<string, object>> dataFilter,
            List<ReportTableColumnModel> tableColumns, List<ReportTableColumnModel> allTableColumns, DynamicReportInfoModel selectDynamicReportInfo,
            List<ReportJoinInfoModel> selectedJoinList);


        Task<IEnumerable<IDictionary<string, object>>> PopulateFilterColumnsAndQueryAndLoadDataNewReport(LoadDataArgs args,
            DynamicReportFilter dynamicReportFilter, RadzenDataFilter<IDictionary<string, object>> dataFilter,
            List<ReportTableColumnModel> tableColumns, List<ReportTableColumnModel> allTableColumns, DynamicReportInfoModel selectDynamicReportInfo,
            List<ReportJoinInfoModel> selectedJoinList);

        Task PopulateSelectedTableColIdsOnViewChange(DynamicReportPagePropertyVM dynamicReportPagePropertyVM, List<ReportTableColumnModel> allTableColumns, List<GridViewSetupModel> gridViewSetups);

        Type GetTypeByString(TableFieldTypeSm fieldTypeEnum);

        string GetColumnPropertyExpression(string name, Type type);
        string ExtractName(string input);
        string ConvertToTitleCase(string input);
    }
}