
namespace CutOutWiz.Core.Message
{
    public class InternalMessageModel
    {
		public int Id { get; set; }
		public int? SenderContactId { get; set; }
		public int RecipientContactId { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public int? RootMessageId { get; set; }
		public DateTime SendTime { get; set; }
		public bool IsRead { get; set; }
		public bool IsDeleted { get; set; }
		public string Type { get; set; }
		public string ObjectId { get; set; }
	}
}
