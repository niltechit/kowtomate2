using CutOutWiz.Core.Dashboards;

namespace CutOutWiz.Services.Dashboards
{
    public interface IDashboardService
    {
        Task<List<DashboardStatsItem>> GetOperationDashboardStatsByFilter(DashboardFilter filter);
        Task<List<DashboardStatsItem>> GetClientDashboardStatsByFilter(long companyId, DateTime? startDate, DateTime? endDate);

	}
}