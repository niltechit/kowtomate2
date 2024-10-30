using CutOutWiz.Core;
using CutOutWiz.Core.Management;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Management
{
    public interface ITeamMemberService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<TeamMemberListModel>> GetAll();
        Task<List<TeamMemberListModel>> GetTeamMembersByContactId(int contactId);
        Task<List<TeamMemberListModel>> GetTeamMemberListWithDetailsByTeamId(int teamId);
        Task<TeamMember> GetById(int teamMemberId);
        Task<TeamMember> GetByObjectId(string objectId);
        Task<List<TeamMember>> GetTeamIdsByContactIdAndOrderId(int contactId, long orderId);
        Task<Response<int>> Insert(TeamMember teamMember);
        Task<Response<bool>> Update(TeamMember teamMember);
        Task<List<int>> GetTeamIdsByContactId(int contactId);
        Task<List<TeamMemberListModel>> GetTeamActiveMemberListWithDetailsByTeamId(int teamId);
        Task<Response<List<int>>> InsertSupportingMember(List<TeamMember> teamMemberList);
        Task<Response<int>> CancelSupportingMember(ContactModel contact, int teamId);
        Task<Response<TeamMember>> GetSupportingMember(ContactModel contact, int teamId);
    }
}
