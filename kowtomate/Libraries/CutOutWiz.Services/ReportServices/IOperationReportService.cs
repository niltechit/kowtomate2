using CutOutWiz.Services.Models.Dashboard;

namespace CutOutWiz.Services.ReportServices
{
    public interface IOperationReportService
    {
        Task<List<MonthlyImageCountModel>> GetMonthlyImagesCompletedDataFroGraph(int companyId, DateTime CurrentDate, int previousNoOfMonths);
        Task<List<MonthlyImageCountModel>> GetImagesReceivedAndCompletedData(int companyId, DateTime startDate, DateTime endDate);
        Task<OperationSummaryReportModel> GetImagesReceivedAndCompletedDataByDatetimeRange(int companyId, DateTime startDate, DateTime endDate);
    }
}