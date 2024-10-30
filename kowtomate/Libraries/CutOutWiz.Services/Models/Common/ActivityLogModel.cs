
namespace CutOutWiz.Core
{
    public class ActivityLogModel
    {
        public int Id { get; set; }
        public byte ActivityLogFor { get; set; }
        public int PrimaryId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int  CreatedByContactId { get; set; }
        public string ObjectId { get; set; }
        public string ContactObjectId { get; set; }
        public string CompanyObjectId { get; set; }
        public byte Category { get; set; }
        public byte Type { get; set; }
    }
}
