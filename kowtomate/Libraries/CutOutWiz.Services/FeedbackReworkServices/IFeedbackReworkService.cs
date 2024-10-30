using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Feedback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.FeedbackReworkServices
{
    public interface IFeedbackReworkService
    {
        Task<Response<int>> Insert(FeedbackOrderItemModel feedbackOrderItem);
        Task<Response<int>> Update(FeedbackOrderItemModel feedbackOrderItem);
        Task<List<ClientOrderItemModel>> GetAllByClientOrderId(long ClientOrderId);
    }
}
