using System.ComponentModel.DataAnnotations;

namespace KowToMateAdmin.Models.Security
{
    public class ContactViewModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name is too long.")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? DesignationId { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [StringLength(100, ErrorMessage = "Email is too long.")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        //[Required(ErrorMessage = "Phone number is required")]
        //[RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
        public string Phone { get; set; }
        public string ProfileImageUrl { get; set; }
        public int? Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedByContactId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedByContactId { get; set; }
        public string ObjectId { get; set; }
        //[Required(ErrorMessage = "Employee ID is required")]
        public string EmployeeId { get; set; }
        public int? TeamId { get; set; }
        //[Required(ErrorMessage = "UserName is required.")]
        public string UserName { get; set; }
        [StringLength(50, MinimumLength = 4, ErrorMessage = "minimum 4 characters required!")]
        //[Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        //[Required(ErrorMessage = "Confirm Password is required.")]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        public string DownloadFolderPath { get; set; }
        public bool IsSharedFolderEnable { get; set; }
        public bool IsUserActive { get; set; }
        public string TeamName { get; set; }
        public IEnumerable<string> SelectedUserRoles { get; set; } = new List<string>();

        //automation need this variable
        public string QcPcCompletedFilePath { get; set; }

    }
}
