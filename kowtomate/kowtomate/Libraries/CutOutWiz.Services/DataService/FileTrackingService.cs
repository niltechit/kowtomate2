using CutOutWiz.Data;
using CutOutWiz.Services.DbAccess;
using Dapper;
using System.Data;

namespace CutOutWiz.Services.DataService
{
    public class FileTrackingService : IFileTrackingService
    {
        private readonly ISqlDataAccess _db;

        public FileTrackingService(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<List<FileTracking>> GetAllFileTracking()
        {
            return await _db.LoadDataUsingProcedure<FileTracking, dynamic>(storedProcedure: "dbo.spFileTracking_GetAll", new { });
        }

        /// <summary>
        /// Get files by company id and action type
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public async Task<List<FileTracking>> GetFileTrackingByCompanyId(int companyId, string actionType)
        {
            return await _db.LoadDataUsingProcedure<FileTracking, dynamic>(storedProcedure: "dbo.spFileTracking_GetIssuesByCompanyId", new { CompanyId = companyId, ActionType = actionType });
        }

        public Task<List<FileTrackingDetails>> GetFileTrackingDetails(int companyId) =>
            _db.LoadDataUsingProcedure<FileTrackingDetails, dynamic>(storedProcedure: "dbo.spFileTrackingDetails_GetAll", new { CompanyId = companyId });

        public Task InsertFileTracking(DataTable fileTrackingType) =>
        _db.SaveDataUsingProcedure(storedProcedure: "dbo.sp_FileTracking_Insert", new
        {
            fileTracking = fileTrackingType.AsTableValuedParameter("FileTrackingType")
        });

        public Task InsertFileTrackingDetails(FileTrackingDetails details) =>
         _db.SaveDataUsingProcedure(storedProcedure: "dbo.sp_FileTrackingDetails_Insert", new
         {
             details.FileTrackingId,
             details.FileName,
             details.FilePathUrl,
             details.FileSize,
             details.FileMarkUp
         });

    }
}
