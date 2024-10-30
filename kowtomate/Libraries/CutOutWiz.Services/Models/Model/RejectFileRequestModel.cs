namespace CutOutWiz.Services.Models.Model
{
    public class RejectFileRequestModel
    {
        public string Base64 { get; set; }
        public string DrivePath { get; set; }
        public string Comment { get; set; }
        public int OrderId { get; set; }
        public string ImageName { get; set; }
        public int OrderItemId { get; set; }
        public int CreatedContactId { get; set; }
    }
}
