
using Microsoft.AspNetCore.Components.Forms;

namespace CutOutWiz.Services.Models.ClientOrders
{
    public class ClientOrderItemModel
    {
        public long Id { get; set; }
        public int CompanyId { get; set; }
        public long ClientOrderId { get; set; }
        public string FileName { get; set; }
        public string ExteranlFileInputPath { get; set; }
        public string ExternalFileOutputPath { get; set; }
        public string InternalFileInputPath { get; set; }
        public string InternalFileOutputPath { get; set; }
        public string ProductionDoneFilePath { get; set; }
        public decimal? UnitPrice { get; set; }
        public byte? Status { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByContactId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedByContactId { get; set; }
        public string ObjectId { get; set; }
        public string FileType { get; set; }
        public long? FileSize { get; set; }
        public string FileByteString { get; set; }
        public string ProductionFileByteString { get; set; }
        public string EditorFirstName { get; set; }
        public string EditorLastName { get; set; }
        public string EmployeeId { get; set; }
        public string EditorName => string.IsNullOrEmpty(EmployeeId)
                                ? $"N/A"
                                : $"{EditorFirstName} {EditorLastName} ({EmployeeId})";
        public int EditorContactId { get; set; }
        public string EditorDownloadFolderPath { get; set; } //new added

        public DateTime? OrderAssignDate { get; set; }
        public int? TeamId { get; set; }
        public byte? ExternalStatus { get; set; }
        public int ExternalStatuss { get; set; }
        public IBrowserFile File { get; set; }
        public string TeamName { get; set; }
        public string PartialPath { get; set; }
        public string FileNameWithoutExtension { get; set; }

		public int? FileGroup { get; set; }
		public DateTime? ArrivalTime { get; set; }

		public int? TimeLeft { get; set; }

		//Other Field
		public string TempFilePathForDuplicateCheck { get; set; }

        public string QueryFilePath { get; set; }
        public bool IsExtraOutPutFile { get; set; }

		public int TotalImageCount { get; set; }
        public string IbrProcessedImageUrl { get; set; }
        public int IbrStatus { get; set; }

        public string FeedBackImagePath { get; set; }
        public DateTime RejectedDate { get; set; }

        public DateTime? ExpectedDeliveryDate { get;set; }

        public int? CategoryId { get; set; }
        public int? CategorySetByContactId { get; set; }
        public DateTime? CategorySetDate { get; set; }
        public string CategoryPrice { get; set; }
        public decimal? TimeInMinute { get; set; }
        public byte? CategorySetStatus { get; set; }
        public int CategoryApprovedByContactId { get; set; }

        public string AttachmentFileByteStrings { get; set; }
        public string CategoryName { get; set; }

        public string RawImageUrl { get; set; }

        public string QcCompletedFilePath { get;set; }

    }


    public class ClientOrderItemListModel
    {
		public long Id { get; set; }
		public long OrderId { get; set; }
        public int CompanyId { get; set; }
		public long ClientOrderId { get; set; }
		public string CompanyObjectId { get; set; }
        public string CompanyName { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderPlaceDate { get; set; }
        public string OrderObjectId { get; set; }
        public DateTime OrderPlaceDateOnly { get; set; }
        
        public string TeamName { get; set; }
        public DateTime? TeamAssignedDate { get; set; }
        public long ClientOrderItemId { get; set; }
        public string FileName { get; set; }
        public string ExteranlFileInputPath { get; set; }
        public string ExternalFileOutputPath { get; set; }
        public string InternalFileInputPath { get; set; }
        public string InternalFileOutputPath { get; set; }
        public byte? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByContactId { get; set; }
        public string ObjectId { get; set; }
        public long? FileSize { get; set; }
        public DateTime? DistributedTime { get; set; }
        public string DistributedByContactId { get; set; }
        public DateTime? DeadlineTime { get; set; }
        public DateTime? InProductionTime { get; set; }
        public DateTime? ProductionDoneTime { get; set; }
        public DateTime? InQcTime { get; set; }
        public int? QcByContactId { get; set; }
        public int? RejectCount { get; set; }
        public int? TeamId { get; set; }
        public byte? ExternalStatus { get; set; }
        public string ProductionDoneFilePath { get; set; }
        public string PartialPath { get; set; }
        public string FileNameWithoutExtension { get; set; }
        public byte? FileGroup { get; set; }
        public bool IsExtraOutPutFile { get; set; }
        public DateTime? ArrivalTime { get; set; }

        public string EditorName { get; set; }
        public int? AssignContactId { get; set; }
	    public int ? AssignByContactId { get; set; }
        public string AssignByName { get; set; }
        public DateTime? AssignToEditorDate { get; set; }

        public int TotalImageCount { get; set; }
        public string RootFolder { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string CategoryName { get; set; }
        public int? CategoryId { get; set; }
        public int? CategorySetByContactId { get; set; }
        public DateTime? CategorySetDate { get; set; }
        public string CategoryPrice { get; set; }
        public decimal? TimeInMinute { get; set; }
        public byte? CategorySetStatus { get; set; }
        public int CategoryApprovedByContactId { get; set; }

    }
}
