using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Core.OrderTeams;

namespace CutOutWiz.Services.OrderTeamServices
{
    public interface IOrderTeamService
    {
        Task<Response<int>> Insert(List<OrderTeamModel> orderTeams, int? orderId);
        Task<OrderTeamModel> GetByOrderIdAndTeamId(int OrderId, int TeamId);
        Task<List<OrderTeamModel>> GetByOrderId(int OrderId);
        Task<Response<bool>> Update(OrderTeamModel orderTeam);
        Task<Response<int>> AssignOrderToTeam(ClientOrderModel order);

	}
}