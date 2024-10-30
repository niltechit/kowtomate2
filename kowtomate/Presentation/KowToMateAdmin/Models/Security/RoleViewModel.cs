using System.ComponentModel.DataAnnotations;

namespace KowToMateAdmin.Models.Security
{
    public class RoleViewModel
    {
        public short Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name is too long.")]
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
