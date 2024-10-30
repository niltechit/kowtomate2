
namespace CutOutWiz.Services.Models.Email
{
    public class ArchiveQueueEmailModel
    {
        public int Id { get; set; }
        public int EmailAccountId { get; set; }
        public string ToEmail { get; set; }
        public string ToEmailName { get; set; }
        public string FromEmail { get; set; }
        public string FromEmailName { get; set; }
        public string CcEmail { get; set; }
        public string BccEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string AttachmentFilePath { get; set; }
        public string AttachedFileName { get; set; }
        public int FileServerId { get; set; }
        public int SentTries { get; set; }
        public DateTime SentOnDateTimeUtc { get; set; }
        public int EmailPriority { get; set; }
        public string ThreadLock { get; set; }
        public DateTime DateProcessedUtc { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByContactId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string ObjectId { get; set; }
    }
}
