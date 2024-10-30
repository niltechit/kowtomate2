using CutOutWiz.Services.Models.Dashboard;
using CutOutWiz.Services.DbAccess;
using Microsoft.Extensions.Configuration;

namespace CutOutWiz.Services.ReportServices
{
    public class OperationReportService : IOperationReportService
    {
        public readonly IConfiguration _configuration;
        private readonly ISqlDataAccess _db;

        public OperationReportService(IConfiguration configuration, ISqlDataAccess db)
        {
            _configuration = configuration;
            _db = db;
        }

        public async Task<List<MonthlyImageCountModel>> GetMonthlyImagesCompletedDataFroGraph(int companyId, DateTime CurrentDate, int previousNoOfMonths)
        {
            return await _db.LoadDataUsingProcedure<MonthlyImageCountModel, dynamic>("[dbo].[SP_Report_Operation_ImageSummaryReport_ByDateRange]",
                new
                {
                    CompanyId = companyId,
                    CurrentDate = CurrentDate,
                    PreviousNoOfMonths = previousNoOfMonths
                });
        }

        public async Task<List<MonthlyImageCountModel>> GetImagesReceivedAndCompletedData(int companyId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _db.LoadDataUsingProcedure<MonthlyImageCountModel, dynamic>("[dbo].[SP_Report_Operation_ImageDailySummaryReport_ByDateRange_ByLastDate]",
                    new
                    {
                        CompanyId = companyId,
                        StartDate = startDate,
                        EndDate = endDate
                    });
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<OperationSummaryReportModel> GetImagesReceivedAndCompletedDataByDatetimeRange(int companyId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = await _db.LoadDataUsingProcedure<OperationSummaryReportModel, dynamic>("[dbo].[SP_Report_Operation_OverallImageSummaryReport_ByDateRange]",
                    new
                    {
                        CompanyId = companyId,
                        StartDate = startDate,
                        EndDate = endDate
                    });
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
