using CutOutWiz.Data.Models.Dashboard;
using CutOutWiz.Services.DbAccess;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.ReportService
{
    public class ReportService : IReportService
    {
        public readonly IConfiguration _configuration;
        private readonly ISqlDataAccess _db;

        public ReportService(IConfiguration configuration, ISqlDataAccess db)
        {
            _configuration = configuration;
            _db = db;
        }

        public async Task<List<string>> GetCompanyBrands(int companyId)
        {
            return (List<string>) await _db.LoadDataUsingProcedure<string, dynamic>("dbo.spGetBrands", new { CompanyId = companyId});
        }

        public async Task<List<string>> GetCompanyArticles(int companyId)
        {
            return (List<string>)await _db.LoadDataUsingProcedure<string, dynamic>("dbo.spGetArticles", new { CompanyId = companyId });
        }

        public async Task<ImageCount> GetDataForDashboard(DateTime startDate, DateTime endDate, string brand, int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<ImageCount, dynamic>("dbo.spGetSummaryReport", 
                new {
                    StartDate = startDate,
                    EndDate = endDate,
                    Brand = brand,
                    CompanyId = companyId
                });

            return result.FirstOrDefault();
        }

    }
}
