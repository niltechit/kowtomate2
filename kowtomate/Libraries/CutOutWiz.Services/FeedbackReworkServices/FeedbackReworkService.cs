using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Feedback;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Data;

namespace CutOutWiz.Services.FeedbackReworkServices
{
    public class FeedbackReworkService : IFeedbackReworkService
    {
        private readonly ISqlDataAccess _db;
        public FeedbackReworkService(ISqlDataAccess db)
        {
            _db= db;
        }
        public async Task<Response<int>> Insert(FeedbackOrderItemModel feedbackOrderItem)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<int, dynamic>(storedProcedure: "dbo.InsertFeedbackOrderItem", new
                {
                   feedbackOrderItem.ClientOrderItemId, 
                   feedbackOrderItem.ClientOrderId,
                   feedbackOrderItem.Comment,
                   feedbackOrderItem.CreatedDate,
                   feedbackOrderItem.FeedBackImagePath,
                   feedbackOrderItem.CreatedById
                });
            
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

        public async Task<Response<int>> Update(FeedbackOrderItemModel feedbackOrderItem)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<int, dynamic>(storedProcedure: "dbo.UpdateFeedbackOrderItem", new
                {
                    feedbackOrderItem.ClientOrderItemId,
                    feedbackOrderItem.ClientOrderId,
                    feedbackOrderItem.Comment,
                    feedbackOrderItem.CreatedDate,
                    feedbackOrderItem.FeedBackImagePath,
                    feedbackOrderItem.CreatedById
                });

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

        public async Task<List<ClientOrderItemModel>> GetAllByClientOrderId(long ClientOrderId)
        {
            return await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.GetFeedbackByClientOrderId", new { ClientOrderId= ClientOrderId });
        }
    }
}
