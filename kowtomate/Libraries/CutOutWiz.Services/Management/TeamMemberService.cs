using CutOutWiz.Core;
using CutOutWiz.Core.Management;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Data;

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
        public async Task<List<TeamMemberListModel>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<TeamMemberListModel, dynamic>(storedProcedure: "dbo.SP_Management_TeamMember_GetAll", new { });
        }

        public async Task<List<TeamMemberListModel>> GetTeamMembersByContactId(int contactId)
        {
            return await _db.LoadDataUsingProcedure<TeamMemberListModel, dynamic>(storedProcedure: "dbo.SP_Management_Team_GetByContactId", new {ContactId = contactId });
        }

        /// <summary>
        /// Get All Menus
        /// </summary>
        /// <returns></returns>
        public async Task<List<TeamMemberListModel>> GetTeamMemberListWithDetailsByTeamId(int teamId)
        {
            return await _db.LoadDataUsingProcedure<TeamMemberListModel, dynamic>(storedProcedure: "dbo.SP_Management_TeamMember_GetListWithDetails", new { TeamId = teamId }); //Todo:Rakib Sp get data by team id and role id 
        }
		public async Task<List<TeamMemberListModel>> GetTeamActiveMemberListWithDetailsByTeamId(int teamId)
		{
			return await _db.LoadDataUsingProcedure<TeamMemberListModel, dynamic>(storedProcedure: "dbo.SP_Management_ActiveTeamMember_GetListWithDetails", new { TeamId = teamId }); //Todo:Rakib Sp get data by team id and role id 
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

        public async Task<List<TeamMember>> GetTeamIdsByContactIdAndOrderId(int contactId, long orderId)
        {
            return await _db.LoadDataUsingProcedure<TeamMember, dynamic>(storedProcedure: "dbo.SP_Management_TeamMember_GetListByContactIdAndOrderId", 
                new { ContactId = contactId, OrderId= orderId });
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

        public async Task<Response<List<int>>> InsertSupportingMember(List<TeamMember> teamMemberList)
        {
            var response = new Response<List<int>>();
            try
            {
                List<int> newEntryIds = new List<int>();


                foreach (var teamMember in teamMemberList)
                {
                    var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Management_SupportTeamMember_Insert", new
                    {
                        teamMember.ContactId,
                        teamMember.TeamId,
                        teamMember.TeamRoleId,
                        teamMember.CreatedByContactId,
                        teamMember.ObjectId,
                        teamMember.IsSupportingMember
                    });

                    newEntryIds.Add(newId);
                }

                //teamMember.Id = newId;
                response.Result = newEntryIds;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<Response<int>> CancelSupportingMember(ContactModel contact, int teamId)
        {
            var response = new Response<int>();
            try
            {
               var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_CancelSupportingMemberByTeamIdandContactId", new
                    {
                        TeamId = teamId,
                        ContactId= contact.Id,
                    });

                //teamMember.Id = newId;
                //response.Result = newId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<Response<TeamMember>> GetSupportingMember(ContactModel contact, int teamId)
        {
            var response = new Response<TeamMember>();
            try
            {
                var TeamMember = await _db.LoadDataUsingProcedure<TeamMember, dynamic>(storedProcedure: "dbo.SP_GetSupportingMemberByTeamIdandContactId", new
                {
                    TeamId = teamId,
                    ContactId = contact.Id,
                });

                //teamMember.Id = newId;
                response.Result = TeamMember.FirstOrDefault();
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
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

        public async Task<List<int>> GetTeamIdsByContactId(int contactId)
        {
            return await _db.LoadDataUsingProcedure<int, dynamic>(storedProcedure: "dbo.SP_Management_TeamMember_GetTeamIdsByContactId",
               new { ContactId = contactId});
        }
      
    }
}
