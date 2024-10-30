using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.Email
{
	public class EmailTemplateModel
	{
		public short Id { get; set; }

		[Required(ErrorMessage = "Sender Account is required.")]
		public short? SenderAccountId { get; set; }

		[Required(ErrorMessage = "Name is required.")]
		[StringLength(100, ErrorMessage = "Name is too long.")]
		public string Name { get; set; }
		public string AccessCode { get; set; }
		public string FromEmailAddress { get; set; }
		public string BCCEmailAddresses { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public short? Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }

	}

}
