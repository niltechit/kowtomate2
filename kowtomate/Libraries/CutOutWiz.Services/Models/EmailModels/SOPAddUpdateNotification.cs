using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Models.EmailModels
{
    public class SOPAddUpdateNotification: EmailModelBase
    {
        public List<ContactModel> Contacts { get; set; }
        public ContactModel Contact { get; set; }
        public string DetailUrl { get; set; }
        public string MailType { get; set; }
        public string Email { get; set; }
        public string LoginContactName { get; set; }
        public string TemplateName { get;set; }
    }
    public class SignUpNotificationViewModel : EmailModelBase
    {
        public List<ContactModel> Contacts { get; set; }
        public ContactModel Contact { get; set; }
        public string DetailUrl { get; set; }
        public string MailType { get; set; }
        public string Email { get; set; }
        public string LoginContactName { get; set; }
        public string TemplateName { get; set; }
    }
}
