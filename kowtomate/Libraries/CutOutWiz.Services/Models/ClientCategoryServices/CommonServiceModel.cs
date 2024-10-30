using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.ClientCategoryServices
{
    public class CommonServiceModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Service Name is required.")]
        [StringLength(20, ErrorMessage = "The field {0} must be a string with a maximum length of {1}.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Time is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Time must be greater than zero.")]
        public decimal? TimeInMinutes { get; set; }
        //[Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public string? PriceInUSD { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedByUsername { get; set; }
        public decimal DecryptedPrice { get; set; }
        public string TimeIn { get; set; }
        public decimal? TotalMinutes { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
