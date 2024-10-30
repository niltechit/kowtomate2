using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Core.OrderTeams;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Data;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.OrderTeamServices
{
    public class OrderTeamService : IOrderTeamService
    {
        private readonly ISqlDataAccess _db;
        private readonly ICompanyTeamManager _companyTeamService;

        public OrderTeamService(ISqlDataAccess db, ICompanyTeamManager companyTeamService)
        {
            _db = db;
			_companyTeamService = companyTeamService;

		}
        public async Task<Response<int>> Insert(List<OrderTeamModel> orderTeams,int? orderId)
        {
            if (orderId > 0)
            {
                await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Order_Assigned_Team_DeleteByOrderId", new
                {
                    OrderId = orderId
                });
            }
          

            var response = new Response<int>();
            try
            {
                foreach (var orderTeam in orderTeams)
                {
                    var newId = await _db.SaveDataUsingProcedureAndReturnId<int, dynamic>(storedProcedure: "dbo.SP_Order_Assigned_Team_Insert", new
                    {
                        orderTeam.OrderId,
                        orderTeam.TeamId,
                        orderTeam.CreatedBy,
                        orderTeam.IsPrimary,
                        orderTeam.IsItemAssignToTeam
                    });

                }
                //orderTeam.Id = newId;
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

        public async Task<OrderTeamModel> GetByOrderIdAndTeamId(int OrderId,int TeamId)
        {
            var result = await _db.LoadDataUsingProcedure<OrderTeamModel, dynamic>(storedProcedure: "dbo.SP_Order_Assigned_Team_GetByOrderIdAndTeamId", new { OrderId = OrderId,TeamId = TeamId });
             return result.FirstOrDefault();
        }
        public async Task<List<OrderTeamModel>> GetByOrderId(int OrderId)
        {
            var result = await _db.LoadDataUsingProcedure<OrderTeamModel, dynamic>(storedProcedure: "dbo.SP_Order_Assigned_Team_GetByOrderId", new { OrderId = OrderId});
            return result;
        }

        public async Task<Response<bool>> Update(OrderTeamModel orderTeam)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_Assigned_Team_Update", new
                {
                    orderTeam.OrderId,
                    orderTeam.IsItemAssignToTeam,
                    orderTeam.TeamId
                   
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

        //AssignOrderToTeam
        public async Task<Response<int>> AssignOrderToTeam(ClientOrderModel order)
        {
            
			var companyTeams = new List<CompanyTeamModel>();
			companyTeams = await _companyTeamService.GetByCompanyId(order.CompanyId);
			List<OrderTeamModel> teamOrders = new List<OrderTeamModel>();
			foreach (var companyTeam in companyTeams)
            {
				OrderTeamModel orderTeam = new OrderTeamModel
				{
					OrderId = order.Id,
					TeamId = companyTeam.TeamId,
					CreatedBy = AutomatedAppConstant.ContactId, //dummy

				    IsPrimary = true,
					IsItemAssignToTeam = true
				};
				teamOrders.Add(orderTeam);
			}
			var addResponse = await Insert(teamOrders, null);

			var response = new Response<int>();
            return response;
        }

    }
}
