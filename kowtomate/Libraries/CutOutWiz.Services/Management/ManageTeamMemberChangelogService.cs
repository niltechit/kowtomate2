using CutOutWiz.Core;
using CutOutWiz.Core.Management;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Data;

namespace CutOutWiz.Services.Management
{
    public class ManageTeamMemberChangelogService : IManageTeamMemberChangelogService
    {
        private readonly ISqlDataAccess _db;

        public ManageTeamMemberChangelogService(ISqlDataAccess db)
        {
            _db = db;
        }
        public async Task<Response<int>> Insert(List<ManageTeamMemberChangeLogModel> manageTeamMemberChangelogs)
        {
            var response = new Response<int>();
            try
            {
                foreach (var manageTeamMemberChangelog in manageTeamMemberChangelogs)
                {
                    var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_ManageTeamMemberChangelog_Insert", new
                    {
                        manageTeamMemberChangelog.TeamId,
                        manageTeamMemberChangelog.MemberContactId,
                        manageTeamMemberChangelog.AssignByContactId,
                        manageTeamMemberChangelog.AssignTime,
                        manageTeamMemberChangelog.AssignNote,
                        manageTeamMemberChangelog.IsSupportingMember,
                        manageTeamMemberChangelog.ManagementTeamMemberId,
                        manageTeamMemberChangelog.SupportFromTeamId
                    });
                }
              

                //teamRole.Id = newId;
                //response.Result = newId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<Response<int>> Update(ManageTeamMemberChangeLogModel manageTeamMemberChangelog)
        {
            var response = new Response<int>();
            try
            {
                 var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_ManageTeamMemberChangelog_UpdateByManagementTeammemberId", new
                    {
                        manageTeamMemberChangelog.CancelByContactId,
                        manageTeamMemberChangelog.CancelTime,
                        manageTeamMemberChangelog.CancelNote,
                        manageTeamMemberChangelog.ManagementTeamMemberId
                    });

                //teamRole.Id = newId;
                //response.Result = newId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<Response<ManageTeamMemberChangeLogModel>> GetManageTeamMemberChangeLog(int contactId,int teamId,bool isSupporting)
        {
            var response = new Response<ManageTeamMemberChangeLogModel>();
            try
            {
                var managementTeammemberLog = await _db.LoadDataUsingProcedure<ManageTeamMemberChangeLogModel, dynamic>(storedProcedure: "dbo.SP_ManageTeamMemberChangelog_Insert", new
                  {
                       ContactId = contactId,
                       TeamId = teamId,
                       IsSupporting = isSupporting
                  });
             
                response.Result = managementTeammemberLog.FirstOrDefault();
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
