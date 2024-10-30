using System;
using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.ClientCategoryServices
{
    public class ClientCategoryModel
    {
        public int Id { get; set; }
        public int ClientCompanyId { get; set; }
        public int CommonCategoryId { get; set; }
        public decimal? TimeInMinutes { get; set; }
        public string PriceInUSD { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedByUsername { get; set; }
        public string CompanyName { get; set; }
        public string CategoryName { get; set; }
        public string ClientCategoryPrice { get; set; }
        public string CategoryServices { get; set; }
    }

	public class ClientCategoryRequestViewModel
	{
		public int Id { get; set; }
		[Required]
		public int ClientCompanyId { get; set; }
		public int CommonCategoryId { get; set; }
        [Required]
        public IEnumerable<string> CommonServiceId { get; set; } = new List<string>();
        [Required(ErrorMessage = "Time is required.")]
        public decimal? TimeInMinutes { get; set; }
		//[Required(ErrorMessage = "Price is required.")]
		[Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
		public string? PriceInUSD { get; set; }
		public bool IsActive { get; set; } = false;
		public bool IsDeleted { get; set; }
		public DateTime CreatedDate { get; set; }
		public string CreatedByUsername { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public string UpdatedByUsername { get; set; }
		public string CompanyName { get; set; }
		public string CategoryName { get; set; }
		public string CommonServiceReturnId { get; set; }
        public IEnumerable<string> PreviousSelectedServiceIds { get; set; } = new List<string>();
    }
}
