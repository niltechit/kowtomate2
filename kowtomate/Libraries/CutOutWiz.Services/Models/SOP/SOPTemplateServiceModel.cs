namespace CutOutWiz.Services.Models.SOP
{
    public class SOPTemplateServiceModel
    {
        public int Id { get; set; }
        public int SOPTemplateId { get; set; }
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
