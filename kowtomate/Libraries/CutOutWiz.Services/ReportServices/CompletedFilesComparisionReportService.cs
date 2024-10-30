using CutOutWiz.Core.ComparisionReports;
using CutOutWiz.Services.DbAccess;

namespace CutOutWiz.Services.ReportServices
{
    public class CompletedFilesComparisionReportService : ICompletedFilesComparisionReportService
    {
        #region Private Members
        private readonly ISqlDataAccess _db;
        #endregion

        #region Ctor

        public CompletedFilesComparisionReportService(ISqlDataAccess db)
        {
            _db = db;
        }

        #endregion

        #region Sales Analysis Amount And Qty Report

        public async Task<List<AnalysisChartReportModel>> GetDeliveryAnalysisReportAmountData(CompletedFilesComparisonReportFilter filter)
        {
            var platfomsQuery = "";

            if (filter.SelectedCompanyIds != null && filter.SelectedCompanyIds.Any())
            {
                if (filter.SelectedCompanyIds.Count() == 1)
                {
                    if (filter.SelectedCompanyIds.FirstOrDefault() != 100)
                    {
                        platfomsQuery = $" AND oi.CompanyId = {filter.SelectedCompanyIds.FirstOrDefault()} ";
                    }
                }
                else
                {
                    var commaSeperaedIds = string.Join(", ", filter.SelectedCompanyIds);
                    platfomsQuery = $" AND oi.CompanyId IN ({commaSeperaedIds}) ";
                }
            }

            string queryString = @$"
            select Year(oi.CreatedDate) [Year], MONTH(oi.CreatedDate) [Month], SUM(1) Value 
from [dbo].[Order_ClientOrderItem] oi WITH(NOLOCK)
INNER JOIN [dbo].[Order_ClientOrder] co WITH(NOLOCK) on co.Id = oi.ClientOrderId
where oi.Status = 21 OR oi.Status = 26
and oi.CreatedDate  between @StartDate and @EndDate {platfomsQuery}
group by Year(oi.CreatedDate), MONTH(oi.CreatedDate)
order by Year(oi.CreatedDate), MONTH(oi.CreatedDate)";

            return await _db.LoadDataUsingQuery<AnalysisChartReportModel, dynamic>(queryString,
            new
            {
                StartDate = filter.PreviousStartDate,
                EndDate = filter.CurrentEndDate
            });
        }

        public async Task<List<AnalysisChartReportModel>> GetDeliveryAnalysisReportQtyData(CompletedFilesComparisonReportFilter filter)
        {
            var platfomsQuery = "";

            if (filter.SelectedCompanyIds != null && filter.SelectedCompanyIds.Any())
            {
                if (filter.SelectedCompanyIds.Count() == 1)
                {
                    if (filter.SelectedCompanyIds.FirstOrDefault() != 100)
                    {
                        platfomsQuery = $" AND oi.CompanyId = {filter.SelectedCompanyIds.FirstOrDefault()} ";
                    }
                }
                else
                {
                    var commaSeperaedIds = string.Join(", ", filter.SelectedCompanyIds);
                    platfomsQuery = $" AND oi.CompanyId IN ({commaSeperaedIds}) ";
                }
            }

            string queryString = @$"select Year(oi.CreatedDate) [Year], MONTH(oi.CreatedDate) [Month], COUNT(*) Value 
            from [dbo].[Order_ClientOrderItem] oi WITH(NOLOCK)
            INNER JOIN [dbo].[Order_ClientOrder] co WITH(NOLOCK) on co.Id = oi.ClientOrderId
            where oi.Status = 21 OR oi.Status = 26
            and oi.CreatedDate  between @StartDate and @EndDate {platfomsQuery}
            group by Year(oi.CreatedDate), MONTH(oi.CreatedDate)
            order by Year(oi.CreatedDate), MONTH(oi.CreatedDate)";

            return await _db.LoadDataUsingQuery<AnalysisChartReportModel, dynamic>(queryString,
            new
            {
                StartDate = filter.PreviousStartDate,
                EndDate = filter.CurrentEndDate
            });
        }

        public async Task<List<AnalysisChartReportModel>> GetLast30DaysDeliveryAnalysisReportAmountData(CompletedFilesComparisonReportFilter filter)
        {
            var platfomsQuery = "";

            if (filter.SelectedCompanyIds != null && filter.SelectedCompanyIds.Any())
            {
                if (filter.SelectedCompanyIds.Count() == 1)
                {
                    if (filter.SelectedCompanyIds.FirstOrDefault() != 100)
                    {
                        platfomsQuery = $" AND oi.CompanyId = {filter.SelectedCompanyIds.FirstOrDefault()} ";
                    }
                }
                else
                {
                    var commaSeperaedIds = string.Join(", ", filter.SelectedCompanyIds);
                    platfomsQuery = $" AND oi.CompanyId IN ({commaSeperaedIds}) ";
                }
            }

            string queryString = @$"
                select Year(oi.CreatedDate) [Year], Month(oi.CreatedDate) [Month] ,Day(oi.CreatedDate) [Day], SUM(1) Value 
from [dbo].[Order_ClientOrderItem] oi WITH(NOLOCK)
INNER JOIN [dbo].[Order_ClientOrder] co WITH(NOLOCK) on co.Id = oi.ClientOrderId
where oi.Status = 21 OR oi.Status = 26
and oi.CreatedDate  between @StartDate and @EndDate {platfomsQuery} 
group by Year(oi.CreatedDate),Month(oi.CreatedDate), Day(oi.CreatedDate)
order by Year(oi.CreatedDate), Month(oi.CreatedDate), Day(oi.CreatedDate)";

            return await _db.LoadDataUsingQuery<AnalysisChartReportModel, dynamic>(queryString,
            new
            {
                StartDate = filter.PreviousStartDate,
                EndDate = filter.CurrentEndDate
            });
        }

        public async Task<List<AnalysisChartReportModel>> GetLast30DaysDeliveryAnalysisReportQtyData(CompletedFilesComparisonReportFilter filter)
        {
            var platfomsQuery = "";

            if (filter.SelectedCompanyIds != null && filter.SelectedCompanyIds.Any())
            {
                if (filter.SelectedCompanyIds.Count() == 1)
                {
                    if (filter.SelectedCompanyIds.FirstOrDefault() != 100)
                    {
                        platfomsQuery = $" AND oi.CompanyId = {filter.SelectedCompanyIds.FirstOrDefault()} ";
                    }
                }
                else
                {
                    var commaSeperaedIds = string.Join(", ", filter.SelectedCompanyIds);
                    platfomsQuery = $" AND oi.CompanyId IN ({commaSeperaedIds}) ";
                }
            }

            string queryString = @$"
                select Year(oi.CreatedDate) [Year], Month(oi.CreatedDate) [Month] ,Day(oi.CreatedDate) [Day], COUNT(*) Value 
    from [dbo].[Order_ClientOrderItem] oi WITH(NOLOCK)
INNER JOIN [dbo].[Order_ClientOrder] co WITH(NOLOCK) on co.Id = oi.ClientOrderId
    where oi.Status = 21 OR oi.Status = 26
    and oi.CreatedDate  between @StartDate and @EndDate {platfomsQuery} 
    group by Year(oi.CreatedDate),Month(oi.CreatedDate), Day(oi.CreatedDate)
    order by Year(oi.CreatedDate), Month(oi.CreatedDate), Day(oi.CreatedDate)";

            return await _db.LoadDataUsingQuery<AnalysisChartReportModel, dynamic>(queryString,
            new
            {
                StartDate = filter.PreviousStartDate,
                EndDate = filter.CurrentEndDate
            });
        }

        #endregion
    }
}
