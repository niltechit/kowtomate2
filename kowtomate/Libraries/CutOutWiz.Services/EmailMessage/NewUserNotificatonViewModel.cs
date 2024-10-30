namespace CutOutWiz.Services.EmailMessage
{
	public class NewUserNotificatonViewModel
	{
		public int Id { get; set; }
		public int CompanyId { get; set; }
		public string CompanyName { get; set; }
		public int ContactId { get; set; }
		public string ContactName { get; set; }
		public string Username { get; set; }
		public string DetailUrl { get; set; }
		public string ToEmail { get; set; }
		public string ToEmailName { get; set; }
		public string CompanyEmail { get; set; }
		public string CreateByUserName { get; set; }
		public string MailType { get; set; }
	}
}