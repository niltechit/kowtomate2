
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Models.Message
{
    public class InternalMessageNotification
    {
        public List<ContactModel> Contacts { get; set; }
        public ContactModel Contact { get; set; }
        public int SenderContactId { get; set; }
        public string MessageType { get; set; }
        public string TemplateName { get;set; }
        public string OrderNumber { get; set; }
        public string TeamName { get; set; }
    }
}
