using CutOutWiz.Services.Models.SOP;
using Microsoft.AspNetCore.Components.Forms;
using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.Models.ClientOrders
{
    public class ClientOrderModel
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
        public int? UpdatedByContactId { get; set; }
        public string ObjectId { get; set; }
        public int NumberOfImage { get; set; }
        public int AssignedByOpsContactId { get; set; }
        public string Instructions { get; set; }
        public IBrowserFile file { get; set; }
        public int OrderType { get; set; }
        public int? AssignedTeamId { get; set; }
		public DateTime? ArrivalTime { get; set; }
		public IReadOnlyList<IBrowserFile> OrderAttachment { get; set; }
        public List<string> SOPStandardServiceList { get; set; } = new List<string>();
        public List<ClientOrderItemModel> orderItems { get; set; } = new List<ClientOrderItemModel>();
        public List<OrderFileAttachment> orderAttachments { get; set; } = new List<OrderFileAttachment>();
        public List<ClientOrderSOPTemplateModel> SOPTemplateList { get; set; } = new List<ClientOrderSOPTemplateModel>();
        public virtual SOPTemplateModel SOPTemplate { get; set; }

        public string InternalOrderStatusEnumName => Enum.GetName(typeof(InternalOrderStatus), InternalOrderStatus);
        public string ExternalOrderStatusEnumName => Enum.GetName(typeof(ExternalOrderStatus), ExternalOrderStatus);

        public bool AllowExtraOutputFileUpload { get; set; }
        public long? SourceServerId { get; set; }
        public int? DeliveryDeadlineInMinute { get; set; }
        public string BatchPath { get; set; }
        public byte? CategorySetStatus { get; set; }
    }

    public class ClientOrderListModel
    {
        public long Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyObjectId { get; set; }
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
        public int? UpdatedByContactId { get; set; }
        public string ObjectId { get; set; }
        public int NumberOfImage { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string TeamName { get; set; }
        public string Ammenment { get; set; }
        public DateTime? TeamAssignedDate { get; set; }
        public string OrderTypeName { get; set; }
        public int TotalCount { get; set; }
        public int OrderType { get; set; }
        public DateTime? OrderPlaceDateOnly { get; set; }
        public int AssignedByOpsContactId { get; set; }
        public int? AssignedTeamId { get; set; }
        public string InternalOrderStatusEnumName => Enum.GetName(typeof(InternalOrderStatus), InternalOrderStatus);
        public string ExternalOrderStatusEnumName => Enum.GetName(typeof(ExternalOrderStatus), ExternalOrderStatus);

		public bool AllowExtraOutputFileUpload { get; set; }

        public int TotalImageCount { get; set; }

        //Filter
		public string ImageName { get; set; }
		public string InternalFileInputPath { get; set; }

		public DateTime? ArrivalTime { get; set; }
		public int? DeliveryDeadlineInMinute { get; set; }
		public int? CompanyDeliveryDeadlineInMinute { get; set; }
		public string TimeLeft { get; set; }
		public string NoMapped { get; set; }
		public string SourceFtpUsername {get;set;}
        public string BatchPath {get;set;}
        public byte? CategorySetStatus { get; set; }
    }
}