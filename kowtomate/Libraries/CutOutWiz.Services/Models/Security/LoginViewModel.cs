
using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.Security
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(100, ErrorMessage = "Username must be less than 100 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MaxLength(100, ErrorMessage = "Username must be less than 100 characters.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

    }
}
