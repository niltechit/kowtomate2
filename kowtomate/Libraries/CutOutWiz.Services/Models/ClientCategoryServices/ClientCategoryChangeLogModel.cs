
namespace CutOutWiz.Services.Models.ClientCategoryServices
{
    public class ClientCategoryChangeLogModel
    {
        public int Id { get; set; }
        public int ClientCategoryId { get; set; }
        public int CategorySetByContactId { get; set; }
        public DateTime CategorySetDate { get; set; }
        public long ClientOrderItemId { get; set; }
    }
}
