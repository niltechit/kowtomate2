using CutOutWiz.Data;
using CutOutWiz.Data.Management;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Management
{
    public class TeamMemberService : ITeamMemberService
    {
        private readonly ISqlDataAccess _db;

        public TeamMemberService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All TeamMembers
        /// </summary>
        /// <returns></returns>
        public async Task<List<TeamMember>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<TeamMember, dynamic>(storedProcedure: "dbo.SP_Management_TeamMember_GetAll", new { });
        }

        /// <summary>
        /// Get All Menus
        /// </summary>
        /// <returns></returns>
        public async Task<List<TeamMemberListModel>> GetListWithDetails(int teamId)
        {
            return await _db.LoadDataUsingProcedure<TeamMemberListModel, dynamic>(storedProcedure: "dbo.SP_Management_TeamMember_GetListWithDetails", new { TeamId = teamId });
        }

        /// <summary>
        /// Get teamMember by teamMember Id
        /// </summary>
        /// <param name="TeamMemberId"></param>
        /// <returns></returns>
        public async Task<TeamMember> GetById(int teamMemberId)
        {
            var result = await _db.LoadDataUsingProcedure<TeamMember, dynamic>(storedProcedure: "dbo.SP_Management_TeamMember_GetById", new { TeamMemberId = teamMemberId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="TeamMemberId"></param>
        /// <returns></returns>
        public async Task<TeamMember> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<TeamMember, dynamic>(storedProcedure: "dbo.SP_Management_TeamMember_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert teamMember
        /// </summary>
        /// <param name="teamMember"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(TeamMember teamMember)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Management_TeamMember_Insert", new
                {
                    teamMember.ContactId,
                    teamMember.TeamId,
                    teamMember.TeamRoleId,
                    teamMember.CreatedByContactId,
                    teamMember.ObjectId,
                });

                teamMember.Id = newId;
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
        /// Update TeamMember
        /// </summary>
        /// <param name="teamMember"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(TeamMember teamMember)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Management_TeamMember_Update", new
                {
                    teamMember.Id,
                    teamMember.ContactId,
                    teamMember.TeamId,
                    teamMember.TeamRoleId,
                    teamMember.UpdatedByContactId
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
        /// Delete TeamMember by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Management_TeamMember_Delete", new { ObjectId = objectId });
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
