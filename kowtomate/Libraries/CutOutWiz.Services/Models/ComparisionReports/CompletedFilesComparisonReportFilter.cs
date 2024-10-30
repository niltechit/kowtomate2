namespace CutOutWiz.Core.ComparisionReports
{
    public class CompletedFilesComparisonReportFilter : BaseSearchFilter
    {
        public int? SalesYear { get; set; }

        public short? BSExternalPlatformId { get; set; }
        public DateTime CurrentStartDate { get; set; }
        public DateTime CurrentEndDate { get; set; }
        public DateTime PreviousStartDate { get; set; }
        public DateTime PreviousEndDate { get; set; }

        public DateTime CurrentLast30StartDate { get; set; }
        public DateTime CurrentLast30EndDate { get; set; }
        public DateTime PreviousLast30StartDate { get; set; }
        public DateTime PreviousLast30EndDate { get; set; }

        public List<int?> SelectedCompanyIds { get; set; }
    }
}
