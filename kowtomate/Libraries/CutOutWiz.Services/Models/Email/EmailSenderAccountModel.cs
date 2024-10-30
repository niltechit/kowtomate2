using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.Email
{
	public class EmailSenderAccountModel
	{
		public short Id { get; set; }

		[Required(ErrorMessage = "Name is required.")]
		[StringLength(100, ErrorMessage = "Name is too long.")]
		public string Name { get; set; }

		//[DataType(DataType.EmailAddress)]
		//[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email")]
		public string Email { get; set; }
		public string EmailDisplayName { get; set; }
		public string MailServer { get; set; }
		public short? Port { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string ApiKey { get; set; }
		public string SecretKey { get; set; }
		public string Domain { get; set; }

		[Required(ErrorMessage = "SSL is required.")]
		public bool EnableSSL { get; set; }

		[Required(ErrorMessage = "Default Credential is required.")]
		public bool UseDefaultCredentials { get; set; }

		[Required(ErrorMessage = "Default selection is required.")]
		public bool IsDefault { get; set; }
		public byte? Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }

	}
}
