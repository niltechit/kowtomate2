using CutOutWiz.Core.ComparisionReports;

namespace CutOutWiz.Services.ReportServices
{
    public interface ICompletedFilesComparisionReportService
    {
        Task<List<AnalysisChartReportModel>> GetLast30DaysDeliveryAnalysisReportAmountData(CompletedFilesComparisonReportFilter filter);
        Task<List<AnalysisChartReportModel>> GetLast30DaysDeliveryAnalysisReportQtyData(CompletedFilesComparisonReportFilter filter);
        Task<List<AnalysisChartReportModel>> GetDeliveryAnalysisReportAmountData(CompletedFilesComparisonReportFilter filter);
        Task<List<AnalysisChartReportModel>> GetDeliveryAnalysisReportQtyData(CompletedFilesComparisonReportFilter filter);
    }
}