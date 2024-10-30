
using Microsoft.AspNetCore.Components.Forms;

namespace CutOutWiz.Services.Models.ClientOrders
{
    public class OrderFileAttachment
    {
        public int Id { get; set; }
        public long? Order_ClientOrder_Id { get; set; }
        public long? CompanyId { get; set; }
        public string FileName { get; set; }
        public string ExternalFileOutPutPath { get; set; }
        public string ExternalFileInputPath { get; set; }
        public string InternalFileInputPath { get; set; }
        public string InternalFileOutPutPath { get; set; }
        public bool? IsDeleted { get; set; }
        public byte? Status { get; set; }
        public DateTime? CreateDated { get; set; }
        public int? CreatedByContactId { get; set; }
        public int? UpdatedByContactId { get; set; }
        public string ObjectId { get; set; }
        public string FileByteString { get; set; }
        public IBrowserFile File { get; set; }
        public string FileType { get; set; }
        public byte FileSize { get; set; }
        public string PartialPath { get; set; }
    }
}
