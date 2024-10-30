
namespace CutOutWiz.Services.Models.Security
{
    public class RoleModel
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public byte? Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByContactId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedByContactId { get; set; }
        public string ObjectId { get; set; }
        public string CompanyObjectId { get; set; }
        public bool IsFixed { get; set; }
    }
}
