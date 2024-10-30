using CutOutWiz.Core.Message;
using CutOutWiz.Services.Models.Message;

namespace CutOutWiz.Services.InternalMessage
{
    public class InternalMessageTokenProvider : IInternalMessageTokenProvider
    {
        public void AddSopAddUpdateInternalMessageForClients(List<InternalMessageToken> tokens, InternalMessageNotification internalMessageNotification)
        {
            tokens.Add(new InternalMessageToken("LoginUser.FirstName", internalMessageNotification.Contact.FirstName));
            tokens.Add(new InternalMessageToken("SOP.TemplateName", internalMessageNotification.TemplateName));
            tokens.Add(new InternalMessageToken("CurrentDateTime", DateTime.Now.ToString()));
        }
        public void AddOrderAddUpdateInternalMessageForClients(List<InternalMessageToken> tokens, InternalMessageNotification internalMessageNotification)
        {
            tokens.Add(new InternalMessageToken("Order.Number", internalMessageNotification.OrderNumber));
        }
        public void AssignOrderToTeamInternalMessageForOpeartion(List<InternalMessageToken> tokens, InternalMessageNotification internalMessageNotification)
        {
            tokens.Add(new InternalMessageToken("Order.Number", internalMessageNotification.OrderNumber));
            tokens.Add(new InternalMessageToken("TeamName", internalMessageNotification.TeamName));
        }
    }
}
