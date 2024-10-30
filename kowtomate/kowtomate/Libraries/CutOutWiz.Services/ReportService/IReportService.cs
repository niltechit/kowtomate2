using CutOutWiz.Data.Models.Dashboard;

namespace CutOutWiz.Services.ReportService
{
    public interface IReportService
    {
        Task<ImageCount> GetDataForDashboard(DateTime startDate, DateTime endDate, string brand, int companyId);

        Task<List<string>> GetCompanyBrands(int companyId);
        Task<List<string>> GetCompanyArticles(int companyId);
    }
}