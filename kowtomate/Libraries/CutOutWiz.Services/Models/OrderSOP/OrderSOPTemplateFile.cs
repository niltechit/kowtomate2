using Microsoft.AspNetCore.Components.Forms;

namespace CutOutWiz.Services.Models.OrderSOP
{
    public class OrderSOPTemplateFile
    {
        public int Id { get; set; }

       public int OrderSOPTemplateId { get; set; }
        public int? BaseSOPTemplateFileId { get; set; }
        public int? BaseTemplateId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }

        public string ActualPath { get; set; }
        public string  ModifiedPath { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByContactId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedByContactId { get; set; }

        public int? FileModifiedByContactId { get; set; }   

        public string ObjectId { get; set; }
        public string RootFolderPath { get; set; }
        public string ViewPath { get; set; }
        public string FileByteString { get; set; }

        public IBrowserFile File { get; set; }

    }
}
