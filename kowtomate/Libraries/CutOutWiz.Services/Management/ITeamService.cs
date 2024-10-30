using CutOutWiz.Core;
using CutOutWiz.Core.Management;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Management
{
    public interface ITeamService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<TeamModel>> GetAll();
        Task<TeamModel> GetTeamName(string objectId);
        Task<TeamModel> GetById(int teamId);
        Task<ContactListModel> GetByContactId(int contactId);
        Task<TeamModel> GetByObjectId(string objectId);
        Task<Response<int>> Insert(TeamModel team);
        Task<Response<bool>> Update(TeamModel team);
        Task<List<TeamModel>> GetByOrderId(long orderId);
    }
}
