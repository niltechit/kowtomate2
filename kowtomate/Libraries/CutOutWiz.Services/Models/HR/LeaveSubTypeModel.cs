using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.HR
{
    public class LeaveSubTypeModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Required is Field")]
        public int LeaveTypeId { get; set; }
        [Required(ErrorMessage ="Name is required")]
        [StringLength(20,ErrorMessage = "Name is too long and max character is {1}")]
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public int? CreatedByContactId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedByContactId { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //Additional Value
        public string LeaveTypeName { get;set; }
        public bool IsDeleted { get; set; }
    }
}
