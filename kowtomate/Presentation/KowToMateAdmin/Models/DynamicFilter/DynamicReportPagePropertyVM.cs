using CutOutWiz.Services.Models.DynamicReports;
using CutOutWiz.Services.Models.UI;
using Radzen.Blazor;

namespace KowToMateAdmin.Models.DynamicFilter
{
    public class DynamicReportPagePropertyVM
    {
        //public RadzenDataGrid<IDictionary<string, object>> grid { get; set; }
        public RadzenDataFilter<IDictionary<string, object>> dataFilter { get; set; }
        public GridViewSetupModel selectedGridViewSetup { get; set; }
        public GridViewFilterModel selectedGridViewFilter { get; set; }
        public List<GridViewFilterModel> gridViewFilters { get; set; }
        public int selectedGridViewSetupId { get; set; } = 0;
        public int selectedGridViewFilterId { get; set; } = 0;
        public IEnumerable<int> selectedTableColumnIds { get; set; } = new List<int>();
        public List<ReportTableColumnModel> tableColumns { get; set; } = new List<ReportTableColumnModel>();
    }
}
