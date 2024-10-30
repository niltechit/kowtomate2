namespace CutOutWiz.Data.Entities.Common
{
	public class FileServerEntity
	{
		public short Id { get; set; }
		public byte? FileServerType { get; set; }
		public string Name { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string AccessKey { get; set; }
		public string SecretKey { get; set; }
		public string RootFolder { get; set; }
		public string SshKeyPath { get; set; }
		public string Host { get; set; }
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
