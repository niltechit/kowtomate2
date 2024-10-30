namespace CutOutWiz.Services.Models.ClientCategoryServices
{
    public class CommonCategoryModel
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? TimeInMinutes { get; set; }
        public string? PriceInUSD { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedByUsername { get; set; }
        public short Status { get; set; }
        public string CategoryServices { get; set; }
    }
}
