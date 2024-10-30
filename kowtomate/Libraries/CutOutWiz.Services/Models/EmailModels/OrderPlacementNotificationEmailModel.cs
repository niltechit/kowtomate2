using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Models.EmailModels
{
    public class OrderPlacementNotificationEmailModel : EmailModelBase
    {
        public List<string> EmailAddresses { get; set; }
        public string MailType { get; set; }

        public string FolderName { get; set; }
        public string ArrivalDateTime { get; set; }
        public string DeliveryTime { get; set; }
        public string OrderType { get; set; }
        public string ImageCount { get; set; }
        public string ReceiverName { get; set; }
        public string CompanyName { get; set; }

        public string FtpUserName { get; set; }
        public string BatchName { get; set; }
        public string OrderNumber { get; set; }

        public List<ContactModel> Contacts { get; set; } = new List<ContactModel>();
        public string OrderDetailURL { get; set; }
    }
}
