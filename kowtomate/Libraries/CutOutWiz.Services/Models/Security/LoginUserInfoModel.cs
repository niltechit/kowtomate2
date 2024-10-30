namespace CutOutWiz.Services.Models.Security
{
    public class LoginUserInfoModel
    {
		public int UserId { get; set; }
		public int ContactId { get; set; }
		public string RoleName { get; set; }			
		public string Username { get; set; }
		public string ProfileImageUrl { get; set; }
		public string UserGuid { get; set; }

		//Custom Fields
		public string FirstName { get; set; }
		public int? CompanyId { get; set; }
		public string ClientRootFolderPath { get; set; }
	}
}
