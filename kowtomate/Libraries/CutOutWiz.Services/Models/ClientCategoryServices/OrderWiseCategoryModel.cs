namespace CutOutWiz.Services.Models.ClientCategoryServices
{
    public class OrderWiseCategoryModel
    {
        public int CategoryId { get; set; }
        public int CategorySetByContactId { get; set; }
        public DateTime CategorySetDate { get; set; }
        public string CategoryPrice { get; set; }
        public long OrderId { get; set; } 
        public decimal? TimeInMinute { get; set; }
        public byte CategorySetStatus { get; set; }
        public int CategoryApprovedByContactId { get; set; }
    }

    public class OrderItemWiseCategoryModel
    {
        public int CategoryId { get; set; }
        public long OrderItemId  { get; set; }
        public int CategorySetByContactId { get; set; }
        public DateTime CategorySetDate { get; set; }
        public string CategoryPrice { get; set; }
        public decimal? TimeInMinute { get; set; }
    }
}
