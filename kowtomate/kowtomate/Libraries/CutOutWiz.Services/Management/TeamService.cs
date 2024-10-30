using CutOutWiz.Data;
using CutOutWiz.Data.Management;
using CutOutWiz.Services.DbAccess;

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
        public async Task<List<Team>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<Team, dynamic>(storedProcedure: "dbo.SP_Management_Team_GetAll", new { });
        }

        /// <summary>
        /// Get team by team Id
        /// </summary>
        /// <param name="TeamId"></param>
        /// <returns></returns>
        public async Task<Team> GetById(int teamId)
        {
            var result = await _db.LoadDataUsingProcedure<Team, dynamic>(storedProcedure: "dbo.SP_Management_Team_GetById", new { TeamId = teamId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="TeamId"></param>
        /// <returns></returns>
        public async Task<Team> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<Team, dynamic>(storedProcedure: "dbo.SP_Management_Team_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert team
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(Team team)
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
        public async Task<Response<bool>> Update(Team team)
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

    }
}
