using CutOutWiz.Core;
using CutOutWiz.Core.Management;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Data;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Management
{
    public class TeamService : ITeamService
    {
        private readonly ISqlDataAccess _db;

        public TeamService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All Teams
        /// </summary>
        /// <returns></returns>
        public async Task<List<TeamModel>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<TeamModel, dynamic>(storedProcedure: "dbo.SP_Management_Team_GetAll", new { });
        }
        public async Task<TeamModel> GetTeamName(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<TeamModel, dynamic>(storedProcedure: "dbo.SP_Common_CompanyAndTeam_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get team by team Id
        /// </summary>
        /// <param name="TeamId"></param>
        /// <returns></returns>
        public async Task<TeamModel> GetById(int teamId)
        {
            var result = await _db.LoadDataUsingProcedure<TeamModel, dynamic>(storedProcedure: "dbo.SP_Management_Team_GetById", new { TeamId = teamId });
            return result.FirstOrDefault();
        }
        public async Task<List<TeamModel>> GetByOrderId(long orderId)
        {
            var result = await _db.LoadDataUsingProcedure<TeamModel, dynamic>(storedProcedure: "dbo.SP_Order_AssignedTeam_GetByOrderId", new { OrderId = orderId });
            return result;
        }
        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="TeamId"></param>
        /// <returns></returns>
        public async Task<TeamModel> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<TeamModel, dynamic>(storedProcedure: "dbo.SP_Management_Team_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert team
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(TeamModel team)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Management_Team_Insert", new
                {
                    team.CompanyId,
                    team.Name,
                    team.Status,
                    team.CreatedByContactId,
                    team.ObjectId,
                });

                team.Id = newId;
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
        /// Update Team
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(TeamModel team)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Management_Team_Update", new
                {
                    team.Id,
                    team.CompanyId,
                    team.Name,
                    team.Status,
                    team.UpdatedByContactId
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
        /// Delete Team by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Management_Team_Delete", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<ContactListModel> GetByContactId(int contactId)
        {
            var result = await _db.LoadDataUsingProcedure<ContactListModel, dynamic>(storedProcedure: "dbo.SP_Management_Team_GetByContactId", new { contactId = contactId });
            return result.FirstOrDefault();
        }
    }
}
