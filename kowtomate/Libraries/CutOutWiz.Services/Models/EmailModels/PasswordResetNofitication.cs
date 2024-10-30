namespace CutOutWiz.Services.Models.EmailModels
{
    public class PasswordResetNofitication : EmailModelBase
    {
        public string PasswordResetUrl { get; set; }
        public string TempPassword { get; set; }
        public string PasswordResetToken { get; set; }
        public string ContactFirstName { get; set; }
    }
}
