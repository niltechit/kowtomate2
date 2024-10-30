using CutOutWiz.Data;
using CutOutWiz.Data.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Management
{
    public interface ITeamMemberService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<TeamMember>> GetAll();
        Task<List<TeamMemberListModel>> GetListWithDetails(int teamId);
        Task<TeamMember> GetById(int teamMemberId);
        Task<TeamMember> GetByObjectId(string objectId);
        Task<Response<int>> Insert(TeamMember teamMember);
        Task<Response<bool>> Update(TeamMember teamMember);
    }
}
