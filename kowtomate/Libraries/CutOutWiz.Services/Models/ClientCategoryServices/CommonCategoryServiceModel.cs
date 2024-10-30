using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.ClientCategoryServices
{
    public class CommonCategoryServiceModel
    {
        public int Id { get; set; }
        public int CommonCategoryId { get; set; }
        public int CommonServiceId { get; set; }
        public decimal? TimeInMinutes { get; set; }
        public string? PriceInUSD { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsActive { get; set; }=true;
        public bool IsDeleted { get; set; }
        public string ServiceName { get; set; }
        public string CategoryName { get; set; }
        public IEnumerable<int> ServiceId { get; set; } = new List<int>();
        public string Name { get; set; }
    }
    public class CommonCategoryServiceViewModel
    {
        public int Id { get; set; }
        public int CommonCategoryId { get; set; }
        public int CommonServiceId { get; set; }
        [Required(ErrorMessage = "Time is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Time must be greater than zero.")]
        public decimal? TimeInMinutes { get; set; }
        //[Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public string? PriceInUSD { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsDeleted { get; set; }
        public string ServiceName { get; set; }

        public string CategoryName { get; set; }
        public IEnumerable<int> ServiceId { get; set; } = new List<int>();
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "The field {0} must be a string with a maximum length of {1}.")]
        public string Name { get; set; }
        public List<CommonServiceModel> commonService { get; set; } = new List<CommonServiceModel>();
    }
}
