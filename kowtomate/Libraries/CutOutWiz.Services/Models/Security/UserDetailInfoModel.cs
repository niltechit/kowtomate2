namespace CutOutWiz.Services.Models.Security
{
    public class UserDetailInfoModel
    {
		public int UserId { get; set; }
		public int ContactId { get; set; }
		public int RoleId { get; set; }
		public string RoleName { get; set; }
		public string Username { get; set; }
		public string ProfileImageUrl { get; set; }		
		public int Status { get; set; }
		public DateTime? LastLoginDateUtc { get; set; }		
		public string UserGuid { get; set; }

		//Custom Fields
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; } 

		public string CompanyName { get; set; }
		public int? CompanyId { get; set; }
		public string ClientRootFolderPath { get; set; }
	}
}
