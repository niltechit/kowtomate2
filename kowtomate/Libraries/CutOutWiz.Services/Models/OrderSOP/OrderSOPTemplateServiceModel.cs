namespace CutOutWiz.Services.Models.OrderSOP
{
    public class OrderSOPTemplateServiceModel
    {
        public int Id { get; set; }
        public int BaseTemplateId { get; set; }
        public int BaseSOPServiceId { get; set; }
        public int OrderSOPTemplateId { get; set; }
        public int OrderSOPStandardServiceId { get; set; }
        public int SOPStandardServiceId { get; set; }
        public int Status { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByContactId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedByContactId { get; set; }

        public string ObjectId { get; set; }


    }
}
