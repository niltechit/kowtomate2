using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Security;
using Microsoft.AspNetCore.Components.Forms;

namespace CutOutWiz.Services.Models.ClientOrders
{
    public class ClientOrderViewModel
    {
        public long Id { get; set; }
        public int CompanyId { get; set; }
        public short? FileServerId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? OrderPlaceDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ProcessingCompletedDate { get; set; }
        public DateTime? InternalQcRequestDate { get; set; }
        public DateTime? InternalQcCompleteDate { get; set; }
        public DateTime? ClientQcRequestDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public byte ExternalOrderStatus { get; set; }
        public byte InternalOrderStatus { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByContactId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedByContactId { get; set; }
        public string ObjectId { get; set; }
        public IReadOnlyList<IBrowserFile> OrderAttachment { get; set; }
        public List<string> SOPStandardServiceList { get; set; } = new List<string>();
        public List<ClientOrderItemModel> orderItems { get; set; } = new List<ClientOrderItemModel>();
        public List<ClientOrderSOPTemplateModel> SOPTemplateList { get; set; } = new List<ClientOrderSOPTemplateModel>();

        #region Use for Email Notification
        public List<ContactModel> Contacts { get; set; }
        public ContactModel Contact { get; set; }
        public string DetailUrl { get; set; }
        public string MailType { get; set; }
        public string Email { get; set; }
        public string LoginContactName { get; set; }
        #endregion
    }
}
