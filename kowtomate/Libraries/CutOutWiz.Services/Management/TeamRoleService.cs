using CutOutWiz.Core;
using CutOutWiz.Core.Management;
using CutOutWiz.Data;
using CutOutWiz.Services.DbAccess;

namespace CutOutWiz.Services.Management
{
    public class TeamRoleService : ITeamRoleService
    {
        private readonly ISqlDataAccess _db;

        public TeamRoleService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All TeamRoles
        /// </summary>
        /// <returns></returns>
        public async Task<List<TeamRoleModel>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<TeamRoleModel, dynamic>(storedProcedure: "dbo.SP_Management_TeamRole_GetAll", new { });
        }

        /// <summary>
        /// Get teamRole by teamRole Id
        /// </summary>
        /// <param name="TeamRoleId"></param>
        /// <returns></returns>
        public async Task<TeamRoleModel> GetById(int teamRoleId)
        {
            var result = await _db.LoadDataUsingProcedure<TeamRoleModel, dynamic>(storedProcedure: "dbo.SP_Management_TeamRole_GetById", new { TeamRoleId = teamRoleId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="TeamRoleId"></param>
        /// <returns></returns>
        public async Task<TeamRoleModel> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<TeamRoleModel, dynamic>(storedProcedure: "dbo.SP_Management_TeamRole_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert teamRole
        /// </summary>
        /// <param name="teamRole"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(TeamRoleModel teamRole)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Management_TeamRole_Insert", new
                {
                    teamRole.CompanyId,
                    teamRole.Name,
                    teamRole.Status,
                    teamRole.CreatedByContactId,
                    teamRole.ObjectId,
                });

                teamRole.Id = newId;
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
        /// Update TeamRole
        /// </summary>
        /// <param name="teamRole"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(TeamRoleModel teamRole)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Management_TeamRole_Update", new
                {
                    teamRole.Id,
                    teamRole.CompanyId,
                    teamRole.Name,
                    teamRole.Status,
                    teamRole.UpdatedByContactId
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
        /// Delete TeamRole by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Management_TeamRole_Delete", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

    }
}
