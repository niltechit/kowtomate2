﻿namespace CutOutWiz.Services.Models.Security
{
	public class UserRoleModel
	{
		public int  Id { get; set; }
		public string UserObjectId { get; set; }
		public string RoleObjectId { get; set; }
		public DateTime UpdatedDate { get; set; }
		public int UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }
	}
}
