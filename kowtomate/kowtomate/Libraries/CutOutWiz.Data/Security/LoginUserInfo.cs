using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Data.Security
{
    public class LoginUserInfo
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
