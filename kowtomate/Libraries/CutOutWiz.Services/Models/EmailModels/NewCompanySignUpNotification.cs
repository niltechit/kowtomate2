
namespace CutOutWiz.Services.Models.EmailModels
{
    public class NewCompanySignUpNotification:EmailModelBase
    {
        public string ClientFirstName { get; set; }
        public string ClientLastName { get; set; }
        public string NewCompanyName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DetailUrl { get; set; }

        public string MailType { get; set; }
        public string CompanyEmail { get; set; }
        public int ContactId { get; set; }
        public string CreateByUserName { get; set; }
        public string LoginUrl { get; set; }
    }
}
