using CutOutWiz.Data;
using CutOutWiz.Data.Management;

namespace CutOutWiz.Services.Management
{
    public interface ITeamService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<Team>> GetAll();
        Task<Team> GetById(int teamId);
        Task<Team> GetByObjectId(string objectId);
        Task<Response<int>> Insert(Team team);
        Task<Response<bool>> Update(Team team);
    }
}
