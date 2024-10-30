using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Models.EmailModels
{
    public class OrderAddUpdateNotification : EmailModelBase
    {
        public List<ContactModel> Contacts { get; set; }
        public ContactModel Contact { get; set; }
        public string OrderNumber { get; set; }
        public string DetailUrl { get; set; }
    }
}
