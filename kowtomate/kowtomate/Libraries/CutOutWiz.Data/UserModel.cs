using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Data
{
    public class UserModel
    {		
		public int Id { get; set; }
		public int CompanyId { get; set; }
		public int ContactId { get; set; }
		public int RoleId { get; set; }
		public string Username { get; set; }
		public string ProfileImageUrl { get; set; }
		public string PasswordHash { get; set; }
		public string PasswordSalt { get; set; }
		public string RegistrationToken { get; set; }
		public string PasswordResetToken { get; set; }
		public int Status { get; set; }
		public DateTime? LastLoginDateUtc { get; set; }
		public DateTime? LastLogoutDateUtc { get; set; }
		public DateTime? LastPasswordChangeUtc { get; set; }
		public string CreateFromUserIp { get; set; }
		public DateTime? CreatedDateUtc { get; set; }
		public DateTime? ChangedDateUtc { get; set; }
		public string UserGuid { get; set; }

	}
}
