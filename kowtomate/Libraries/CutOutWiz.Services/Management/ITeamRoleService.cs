using CutOutWiz.Core;
using CutOutWiz.Core.Management;

namespace CutOutWiz.Services.Management
{
    public interface ITeamRoleService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<TeamRoleModel>> GetAll();
        Task<TeamRoleModel> GetById(int teamRoleId);
        Task<TeamRoleModel> GetByObjectId(string objectId);
        Task<Response<int>> Insert(TeamRoleModel teamRole);
        Task<Response<bool>> Update(TeamRoleModel teamRole);
    }
}
