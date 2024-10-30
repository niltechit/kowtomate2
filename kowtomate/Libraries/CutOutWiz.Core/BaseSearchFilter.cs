
namespace CutOutWiz.Core
{
    public class BaseSearchFilter
    {
        public BaseSearchFilter()
        {
            SearchTerm = string.Empty;
            SortColumn = string.Empty;
            SortDirection = string.Empty;

            // defaults
            Skip = 0;
            Top = 20;
            ReturnAllRows = false;
        }

        public int? CompanyId  { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }
        public string SearchTerm { get; set; }
        public string SortDirection { get; set; }
        public string SortColumn { get; set; }

        public int Skip { get; set; }
        public int Top { get; set; }
        public int TotalCount { get; set; }
        public bool ReturnAllRows { get; set; }
        public bool IsSearch { get; set; }
        public bool IsCalculateTotal { get; set; }

        public string Where { get; set; }
        public string ExtraJoin { get; set; }
        public string SelectedColumns { get; set; }
    }
}
