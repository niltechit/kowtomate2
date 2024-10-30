using System.Globalization;

namespace CutOutWiz.Services.Models.EmailModels
{
    public class EmailModelBase
    {
        public string ToEmail { get; set; }
        public string ToEmailName { get; set; }
        public string FromEmail { get; set; }
        public string FromEmailName { get; set; }
        //public string AdminName { get; set; }
        //public string AdminEmail { get; set; }
        public int CompanyId { get; set; }
        public int CreatedByContactId { get; set; }
        public string CompanyName { get; set; }
        public string CopyrightYear { get; set; }
        public string MessageBody { get; set; }

        public EmailModelBase()
        {
            CompanyName = "Kow to mate Admin";
            CopyrightYear = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
        }
    }
}
