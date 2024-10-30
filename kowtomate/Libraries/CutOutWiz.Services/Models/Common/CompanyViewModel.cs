using CutOutWiz.Services.Models.Security;
using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.Common
{
    public class CompanyViewModel
    {
		public int Id { get; set; }
		[Required(ErrorMessage = "Name is required.")]
		[StringLength(100, ErrorMessage = "Name is too long.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Code is required.")]
		[StringLength(6, ErrorMessage = "Code is too long.")]
		public string Code { get; set; }

		[Required(ErrorMessage = "Type is required.")]
		public byte? CompanyType { get; set; }
		public string Telephone { get; set; }
		[DataType(DataType.EmailAddress)]
		[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email")]
		public string Email { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zipcode { get; set; }
		public string Country { get; set; }
		public short Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int? CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }
		public string RoleName { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string ClientContactName { get; set; }
		public virtual ContactModel Contact { get; set; }
		public string DetailUrl { get; set; }
		public string MailType { get; set; }
		public int ContactId { get; set; }
		public virtual UserModel User { get; set; }
	}
}
