namespace CutOutWiz.Services.Models.Security
{
    public class ContactModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? DesignationId { get; set; }
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
        public string Password { get; set; }
        //[Required(ErrorMessage = "Confirm Password is required.")]
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
