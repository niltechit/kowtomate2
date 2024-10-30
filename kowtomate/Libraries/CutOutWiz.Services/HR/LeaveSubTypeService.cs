using CutOutWiz.Core;
using CutOutWiz.Services.Models.HR;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Data;

namespace CutOutWiz.Services.HR
{
    public class LeaveSubTypeService : ILeaveSubTypeService
    {
        private readonly ISqlDataAccess _db;

        public LeaveSubTypeService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All LeaveSubTypes
        /// </summary>
        /// <returns></returns>
        public async Task<List<LeaveSubTypeModel>> GetAll()
        {
            string query = @"SELECT 
                            lst.Name,lst.Id,lst.LeaveTypeId,lst.IsActive,lst.CreatedByContactId,lst.CreatedDate,
                            lst.UpdatedByContactId,lst.UpdatedDate, lt.Name as LeaveTypeName 
                            FROM [dbo].[HR_LeaveSubType] as lst 
                            left join dbo.[HR_LeaveType] as lt on lt.Id = lst.LeaveTypeId where ISNULL(lst.IsDeleted, 0) = 0";
            return await _db.LoadDataUsingQuery<LeaveSubTypeModel, dynamic>(query, new { });
        }

        /// <summary>
        /// Get leaveType by leaveType Id
        /// </summary>
        /// <param name="LeaveSubTypeId"></param>
        /// <returns></returns>
        public async Task<LeaveSubTypeModel> GetById(int id)
        {
            string query = "SELECT *FROM [dbo].[HR_LeaveSubType] WHERE Id= @LeaveSubTypeId";
            var result = await _db.LoadDataUsingQuery<LeaveSubTypeModel, dynamic>(query, 
                new { LeaveSubTypeId = id });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert leaveType
        /// </summary>
        /// <param name="leaveType"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(LeaveSubTypeModel leaveType)
        {
            var response = new Core.Response<int>();
            
            try
            {
                string sql = @"INSERT INTO [dbo].[HR_LeaveSubType] (Name, IsActive, CreatedByContactId, CreatedDate,LeaveTypeId)
                            VALUES (@Name, @IsActive, @CreatedByContactId, @CreatedDate,@LeaveTypeId);
                            SELECT CAST(SCOPE_IDENTITY() as int)";

                var newId = await _db.SaveDataUsingQueryAndReturnId<int, dynamic>(
                    sql, new
                    {
                        Name = leaveType.Name,
                        IsActive = leaveType.IsActive,
                        LeaveTypeId = leaveType.LeaveTypeId,
                        CreatedByContactId = leaveType.CreatedByContactId,
                        CreatedDate = DateTime.Now
                    });

                leaveType.Id = newId;
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

        /// <summary>
        /// Update LeaveSubType
        /// </summary>
        /// <param name="leaveType"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(LeaveSubTypeModel leaveType)
        {
            var response = new Core.Response<bool>();

            try
            {
                string sql = @"UPDATE [dbo].[HR_LeaveSubType] 
                               SET Name = @Name, 
                               IsActive = @IsActive, 
                               LeaveTypeId = @LeaveTypeId,
                               UpdatedByContactId = @UpdatedByContactId, 
                               UpdatedDate = @UpdatedDate 
                                IsDeleted = @IsDeleted
                               WHERE Id = @Id";

                await _db.SaveDataUsingQuery(sql, new
                {
                    Id = leaveType.Id,
                    Name = leaveType.Name,
                    IsActive = leaveType.IsActive,
                    LeaveTypeId = leaveType.LeaveTypeId,
                    UpdatedByContactId = leaveType.UpdatedByContactId,
                    UpdatedDate = leaveType.UpdatedDate,
                    IsDeleted = leaveType.IsDeleted,
                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Delete LeaveSubType by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(int id)
        {
            var response = new Core.Response<bool>();

            try
            {
                string sql = "DELETE FROM [dbo].[HR_LeaveSubType] WHERE ID = @id";

                await _db.SaveDataUsingQuery(sql, new { id = id });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<Response<List<LeaveSubTypeModel>>> GetSubLeaveTypes(int leaveTypeId)
        {
            var response = new Response<List<LeaveSubTypeModel>>();
            try
            {
                string query = "SELECT * FROM HR_LeaveSubType lst\r\n\r\nwhere lst.LeaveTypeId = @leaveTypeId";
                var subLeaveTyspes =  await _db.LoadDataUsingQuery<LeaveSubTypeModel, dynamic>(query, new { leaveTypeId = leaveTypeId });
                response.IsSuccess = true;
                response.Result = subLeaveTyspes;
            }
            catch(Exception ex)
            {
                response = new Response<List<LeaveSubTypeModel>>();
                response.IsSuccess = false;
                response.Message = ex.Message;
                
            }
            return response;
        }
    }
}
