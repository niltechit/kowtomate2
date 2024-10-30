using CutOutWiz.Core;
using CutOutWiz.Data.DbAccess;
using CutOutWiz.Data.Entities.Common;

namespace CutOutWiz.Data.Repositories.Common
{
    public class ShiftRepository: IShiftRepository
	{
        private readonly ISqlDataAccess _db;

        public ShiftRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<Response<int>> Insert(ShiftEntity shift)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Common_Shift_Insert", new
                {
                    shift.Name,
                    shift.Code,
                    shift.StartTime,
                    shift.EndTime
                });

                shift.Id = newId;
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

        public async Task<List<ShiftEntity>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<ShiftEntity, dynamic>(storedProcedure: "dbo.SP_Common_Shift_GetAll", new { });
        }
    }
}
