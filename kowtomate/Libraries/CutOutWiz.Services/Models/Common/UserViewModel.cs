using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Core
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public int ContactId { get; set; }
        public string FirstName { get; set; }
        public string  LastName { get; set; }
        public string UserName { get; set;}
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string RoleName { get; set; }
        public string Password { get; set; }

        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        public string PreviousPassword { get; set; }

        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string RegistrationToken { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime? LastLoginDateUtc { get; set; }
        public DateTime? LastLogoutDateUtc { get; set; }
        public DateTime? LastPasswordChangeUtc { get; set; }
        public int Status { get; set; }
        public string ObjectId { get; set; }

    }
}
