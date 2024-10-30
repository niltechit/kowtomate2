using CutOutWiz.Services.Models.Dashboard;

namespace CutOutWiz.Services.ReportServices
{
    public interface IReportService
    {
        Task<ImageCountModel> GetDataForDashboard(DateTime startDate, DateTime endDate, string brand, int companyId);

        Task<List<string>> GetCompanyBrands(int companyId);
        Task<List<string>> GetCompanyArticles(int companyId);
    }
}