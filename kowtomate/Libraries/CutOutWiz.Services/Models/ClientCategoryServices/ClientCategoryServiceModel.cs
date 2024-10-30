
namespace CutOutWiz.Services.Models.ClientCategoryServices
{
    public class ClientCategoryServiceModel
    {
        public int Id { get; set; }
        public int ClientCategoryId { get; set; }
        public int CommonServiceId { get; set; }
        public decimal? TimeInMinutes { get; set; }
        public string PriceInUSD { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedByUsername { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsDeleted { get; set; }
        public string CategoryName { get; set; }
        public string ServiceName { get; set; }
        public string Name { get; set; }
    }
}
