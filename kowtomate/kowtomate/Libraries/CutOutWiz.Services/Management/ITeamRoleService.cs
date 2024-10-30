using CutOutWiz.Data;
using CutOutWiz.Data.Management;

namespace CutOutWiz.Services.Management
{
    public interface ITeamRoleService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<TeamRole>> GetAll();
        Task<TeamRole> GetById(int teamRoleId);
        Task<TeamRole> GetByObjectId(string objectId);
        Task<Response<int>> Insert(TeamRole teamRole);
        Task<Response<bool>> Update(TeamRole teamRole);
    }
}
