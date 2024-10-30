using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.HR
{
    public class LeaveTypeModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Required the name")]
        [StringLength(20,ErrorMessage = "Name too long and max character {1}")]
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public int? CreatedByContactId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedByContactId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
