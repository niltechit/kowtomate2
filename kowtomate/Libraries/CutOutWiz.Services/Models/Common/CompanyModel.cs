using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.Models.Security;
using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.Common
{
	public class CompanyModel
    {
		public int Id { get; set; }

		[Required(ErrorMessage = "Name is required.")]
		[StringLength(100, ErrorMessage = "Name is too long.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Code is required. Code must be less than 5 character!")]
		[StringLength(6, ErrorMessage = "Code must be less than 5 character!")]
		public string Code { get; set; }
		public byte? CompanyType { get; set; }
		[Required(ErrorMessage = "Phone number is required")]
		[RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
		public string Telephone { get; set; }
		[Required]
		[RegularExpression(@"^[\w-_]+(\.[\w!#$%'*+\/=?\^`{|}]+)*@((([\-\w]+\.)+[a-zA-Z]{2,20})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$", ErrorMessage = "E-mail is not valid 'Example : someone@domain.com'")]
		public string Email { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zipcode { get; set; }
		public string Country { get; set; }
		public string FileServer { get; set; }
		public short Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int? CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }
		public short FileServerId { get; set; }

		// Contact 
		public virtual ContactModel Contact { get; set; }

		[Required]
        [StringLength(50, MinimumLength = 4,ErrorMessage ="minimum 4 characters required!")]
        public string UserName { get; set; }
		public string Password { get; set; }
		[Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
		public string ConfirmPassword { get; set; }
		[Required]
        public string FirstName { get; set; }
		public string LastName { get; set; }
		public int TeamId { get; set; }
		public int UserId { get; set; }
		public int ContactId { get; set; }
		public int? DeliveryDeadlineInMinute { get; set; }
	}
}
