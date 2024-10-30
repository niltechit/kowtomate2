using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.Security
{
    public class NewCompanySignUpModel
    {		
		public int Id { get; set; }
		[Required(ErrorMessage = "Name is required.")]
		[StringLength(100, ErrorMessage = "Name is too long.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Code is required. Code must be less than 5 character!")]
		[StringLength(6, ErrorMessage = "Code must be less than 5 character!")]
		public string Code { get; set; }

		public string Telephone { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [StringLength(100, ErrorMessage = "Email is too long.")]
		[DataType(DataType.EmailAddress)]
		[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email")]
		public string Email { get; set; }

		// Contact 
		//[Required(ErrorMessage = "UserName is required.")]
		//[StringLength(50, ErrorMessage = "UserName is too long.")]
		//public string UserName { get; set; }
		[Required(ErrorMessage = "Password is required.")]
		[StringLength(50, ErrorMessage = "Password is too long.")]
		public string Password { get; set; }
		[Required(ErrorMessage = "Confirm Password is required.")]
		[Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again!")]
		public string ConfirmPassword { get; set; }
		[Required(ErrorMessage = "First Name is required.")]
		[StringLength(100, ErrorMessage = "First Name is too long.")]
		public string FirstName { get; set; }
		public string LastName { get; set; }
        [Required(ErrorMessage = "User Name is required.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "minimum 4 characters required!")]
        public string UserName { get; set; }
		
	}
}
