
using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.Common
{
    public class FileServerModel
    {
		public short Id { get; set; }

		[Required(ErrorMessage = "File server type is required.")]
		public byte? FileServerType { get; set; }

		[Required(ErrorMessage = "Name is required.")]
		[StringLength(100, ErrorMessage = "Name is too long.")]
		public string Name { get; set; }

		[StringLength(100, ErrorMessage = "Name is too long.")]
		public string UserName { get; set; }

		[StringLength(100, ErrorMessage = "Name is too long.")]
		public string Password { get; set; }

		[StringLength(100, ErrorMessage = "Name is too long.")]
		public string AccessKey { get; set; }

		[StringLength(100, ErrorMessage = "Name is too long.")]
		public string SecretKey { get; set; }

		[StringLength(150, ErrorMessage = "Name is too long.")]
		public string RootFolder { get; set; }

		[StringLength(150, ErrorMessage = "Name is too long.")]
		public string SshKeyPath { get; set; }

		[StringLength(150, ErrorMessage = "Name is too long.")]
		public string Host { get; set; }

		[StringLength(10, ErrorMessage = "Name is too long.")]

		public int? Port { get; set; }
		public string Protocol { get; set; }

		public bool IsDefault { get; set; }
		public byte? Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }
		public string SubFolder { get; set; }
	}
}
