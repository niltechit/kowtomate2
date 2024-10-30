namespace CutOutWiz.Core
{
    public class FileTrackingModel
    {
		public int Id { get; set; }
		public int? CompanyId { get; set; }
		public string SourceDrive { get; set; }
		public string ParentDirectory { get; set; }
		public string BucketName { get; set; }
		public DateTime? ActionDate { get; set; }
		public string ActionType { get; set; }
		public string Attachment { get; set; }
		public string Comments { get; set; }
		public string MarkupImageUrl { get; set; }
		public string FileName { get; set; }
		public int? Status { get; set; }
		public int? CreatedByContactId { get; set; }
		public DateTime? CreatedDateUtc { get; set; }
		public string Brand { get; set; }
		public string Article { get; set; }
		public string FileType { get; set; }
	}
}
