using CutOutWiz.Core;
using CutOutWiz.Data;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Services.Logs;

namespace CutOutWiz.Services.Log
{
    public class LogServices : ILogServices
    {
        private readonly ISqlDataAccess _db;

        public LogServices(ISqlDataAccess db)
        {
            _db = db;
        }
        public async Task<List<ActivityLogModel>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<ActivityLogModel, dynamic>(storedProcedure: "dbo.SP_ActivityLog_GetAll", new { });
        }

        public async Task<Response<int>> Insert(ActivityLogModel activityLog)
		{
            var response = new Response<int>();
            try
			{
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_ActivityLog_Insert", new
                {
                    activityLog.ActivityLogFor,
                    activityLog.PrimaryId,
                    activityLog.Description,
                    activityLog.CreatedByContactId,
                    activityLog.ObjectId,
                    activityLog.ContactObjectId,
                    activityLog.CompanyObjectId,
                    activityLog.Category,
                    activityLog.Type
                });
                if (newId == 0)
                {
                    response.Message = StandardDataAccessMessages.ErrorOnAddMessaage;
                    return response;
                }

                activityLog.Id = newId;
                response.Result = newId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;


        }

        public async Task<List<ActivityLogModel>> GetByActivityLogFor(byte activityLogFor,int primaryId)
        {
            var result = await _db.LoadDataUsingProcedure<ActivityLogModel, dynamic>(storedProcedure: "dbo.SP_ActivityLog_GetByActivityLogFor", new { ActivityLogFor = activityLogFor, PrimaryId = primaryId});
            return result.ToList();
        }

        public async Task<List<ActivityLogModel>> GetAllByContactObjectId(string contactObjectId)
        {
            var result = await _db.LoadDataUsingProcedure<ActivityLogModel, dynamic>(storedProcedure: "dbo.SP_ActivityLog_GetAll_By_ContactObjectId", new { contactObjectId=contactObjectId });
            return result.ToList();
        }
    }
}
