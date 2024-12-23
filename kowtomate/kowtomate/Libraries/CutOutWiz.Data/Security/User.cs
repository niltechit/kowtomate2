﻿
using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Data.Security
{
	public class User
	{
		public int Id { get; set; }
		public int CompanyId { get; set; }
		public int ContactId { get; set; }	
		public string Username { get; set; }
		public string ProfileImageUrl { get; set; }
		public string PasswordHash { get; set; }
		public string PasswordSalt { get; set; }
		public string RegistrationToken { get; set; }
		public string PasswordResetToken { get; set; }
		public DateTime? LastLoginDateUtc { get; set; }
		public DateTime? LastLogoutDateUtc { get; set; }
		public DateTime? LastPasswordChangeUtc { get; set; }
		public int Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }

		public IEnumerable<string> SelectedUserRoles { get; set; } = new List<string>();
	}

	public class UserListModel
	{
		public int Id { get; set; }
		public int CompanyId { get; set; }
		public int ContactId { get; set; }

		[Required(ErrorMessage = "Name is required.")]
		[StringLength(100, ErrorMessage = "Name is too long.")]
		public string Username { get; set; }

        public string Password { get; set; }

		[Compare(nameof(Password))]
		public string ConfirmPassword { get; set; }

        public string ProfileImageUrl { get; set; }
		public string PasswordHash { get; set; }
		public string PasswordSalt { get; set; }
		public string RegistrationToken { get; set; }
		public string PasswordResetToken { get; set; }
		public DateTime? LastLoginDateUtc { get; set; }
		public DateTime? LastLogoutDateUtc { get; set; }
		public DateTime? LastPasswordChangeUtc { get; set; }
		public int Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }

		public IEnumerable<string> SelectedUserRoles { get; set; } = new List<string>();
	}
}
