using CutOutWiz.Core.Dashboards;
using CutOutWiz.Services.DbAccess;
namespace CutOutWiz.Services.Dashboards
{
    public class DashboardService : IDashboardService
    {
        private readonly ISqlDataAccess _db;

        public DashboardService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All Designations
        /// </summary>
        /// <returns></returns>
        public async Task<List<DashboardStatsItem>> GetOperationDashboardStatsByFilter(DashboardFilter filter)
        {
            if (string.IsNullOrWhiteSpace(filter.TeamIds))
            {
                filter.TeamIds = null;
            }

            if (string.IsNullOrWhiteSpace(filter.ClientIds))
            {
                filter.ClientIds = null;
            }

            return await _db.LoadDataUsingProcedure<DashboardStatsItem, dynamic>(storedProcedure: "dbo.SP_Dashboard_OperationDashboardStats",
                new
                {
                    StartDate = filter.StartDate,
                    EndDate = filter.EndDate,
                    TeamIds = filter.TeamIds,
                    ClientIds = filter.ClientIds
                });
        }

		public async Task<List<DashboardStatsItem>> GetClientDashboardStatsByFilter(long companyId, DateTime? startDate, DateTime? endDate)
		{
		        return await _db.LoadDataUsingProcedure<DashboardStatsItem, dynamic>(storedProcedure: "dbo.SP_Dashboard_ClientDashboardStats",
				new
				{
					CompanyId = companyId,
                    StartDate= startDate,
                    EndDate = endDate,
                });
		}


	}
}
