using CutOutWiz.Core.Message;
using CutOutWiz.Services.Models.Message;

namespace CutOutWiz.Services.InternalMessage
{
    public interface IInternalMessageTokenProvider
    {
        void AddSopAddUpdateInternalMessageForClients(List<InternalMessageToken> tokens, InternalMessageNotification internalMessageNotification);
        void AddOrderAddUpdateInternalMessageForClients(List<InternalMessageToken> tokens, InternalMessageNotification internalMessageNotification);
        void AssignOrderToTeamInternalMessageForOpeartion(List<InternalMessageToken> tokens, InternalMessageNotification internalMessageNotification);
    }
}