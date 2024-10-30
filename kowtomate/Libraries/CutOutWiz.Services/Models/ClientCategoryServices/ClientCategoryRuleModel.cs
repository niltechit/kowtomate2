using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.ClientCategoryServices
{
    public class ClientCategoryRuleModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public int ClientCategoryBaseRuleId { get; set; }
        public int? Label { get; set; }
        [Required]
        public string Indicator { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select a filter type.")]
        public byte? FilterType { get; set; }
        [Required]
        public decimal? ExecutionOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int CreatedByContactId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int UpdatedByContactId { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
