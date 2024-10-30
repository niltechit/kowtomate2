using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.ClientCategoryServices
{
    public class ClientCategoryBaseRuleModel
    {
        public int Id { get; set; }
        [Required]
        [Range(1,int.MaxValue, ErrorMessage = "Select a company.")]
        public int CompanyId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select a category.")]
        public int ClientCategoryId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByContactId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedByContactId { get; set; }
        public bool? IsDeleted { get; set; }

        public string CompanyName { get; set; }
        public string CategoryName { get; set; }
    }
}
