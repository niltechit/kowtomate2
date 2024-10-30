
using System.ComponentModel;

namespace CutOutWiz.Core.Utilities
{
    public class Enums
    {       
        public enum GeneralStatus
        {
            Active = 1,
            Inactive = 2
        }

        public enum DynamicReportSqlType
        {
            Script = 1,
            Procedure = 2
        }

        public enum GridViewFor
        {
            MainProduct = 1,
            MainEditCmp = 2,
            POMasterReport = 3,
            JSetAwaitingSetNNItems = 4,
            SupportTicket = 5,
            TransferQuanity = 6,
            BSExternalOrder = 7,
            Others = 100
        }

        public enum TableFieldTypeSm
        {
            ShortText = 1,
            //Paragraph = 2,
            //HtmlParagraph = 3,
            Boolean = 4,
            //Dropdown = 5,
            //Multiselect = 6,
            Integer = 7,
            Decimal = 8,
            Date = 9,
        }

        public enum TableFieldType
        {
            ShortText = 1,
            Paragraph = 2,
            HtmlParagraph = 3,
            Boolean = 4,
            Dropdown = 5,
            Multiselect = 6,
            Integer = 7,
            Decimal = 8,
            Date = 9,
            Short = 10
        }

        public enum DynamicReportType
        {
            AdvanceDynamicReport = 1,
            EmailOnlyReport = 2,
            ExcelFormatReport = 3,
            BasicDynamicReport = 4
        }

        //
        // Summary:
        //     Specifies text alignment. Usually rendered as CSS text-align attribute.
        public enum CustomTextAlign
        {
            //
            // Summary:
            //     The text is aligned to the left side of its container.
            Left,
            //
            // Summary:
            //     The text is aligned to the right side of its container.
            Right,
            //
            // Summary:
            //     The text is centered in its container.
            Center,
            //
            // Summary:
            //     The text is justified.
            Justify,
            //
            // Summary:
            //     Same as justify, but also forces the last line to be justified.
            JustifyAll,
            //
            // Summary:
            //     The same as left if direction is left-to-right and right if direction is right-to-left..
            Start,
            //
            // Summary:
            //     The same as right if direction is left-to-right and left if direction is right-to-left..
            End
        }

        public enum OrderColumns
        {
            Company = 1,
            OrderNumber = 2,
            InternalOrderStatus = 3,
            ExternalOrderStatus = 4,
            NumberOfImage = 5,
            OrderPlaceDateOnly = 6,
            AllowExtraOutputFileUpload = 7,
            ContactName = 8,
            TeamName = 9,
            TeamAssignedDate = 10
        }

        public enum SopStatus
        {
            New = 1,
            ReviewPriceByOps = 2,
            ReviewPriceByClient = 3,
            PriceApproved=4,
            RequestByOpsForApprove=5
        }
        public enum FileServerType
        {
            GCP = 1,
            S3 = 2,
            FTP = 3,
        }

        public enum CompanyType
        {
            //System = 1,
            //Operation = 2,
            //Client = 3
            Admin = 1,
            Client = 2
        }
        public enum ExternalOrderStatus
        { 
            OrderPlaced = 1,
            DownLoaded = 2,
            InProgress = 3, // Internal Status 4,5,6

            InQc = 10,
            ReadyToDeliver = 14,

            ReadyToCheck = 16, //If Approval tool Exist 
            ReadyToDownload = 24,  //If Approval tool Have 
            Rejected = 25, // 
            Completed = 26,
            OrderPlacing = 27,
        }

        public enum InternalOrderStatus
        {
           
            OrderPlaced = 1,
            Downloaded = 2,
            Assigned= 4,
            AssignedForSupport = 5,
            Distributed = 7,
            InProgress = 6,

           

            InProduction = 8,
            ProductionDone = 9,
            InQc = 10,

           
            
            ReworkDistributed = 11,
            ReworkInProduction = 12,
            ReworkDone = 13,
            ReworkQc = 14,

            ReadyToDeliver = 20,
            Delivered = 21,
            Rejected = 25, // 
            Completed = 26,
            OrderPlacing = 27,
        }

        public enum InternalOrderItemStatus
        {
            OrderPlaced = 1,
            Assigned = 4,
			AssignedForSupport = 5,
            Distributed = 7,
            
            InProduction = 8,
            ProductionDone = 9,
            
            InQc = 10,

            ReworkDistributed = 11,
            ReworkInProduction= 12,
            ReworkDone = 13,
            ReworkQc = 14,

            ReadyToDeliver =20,
            Delivered = 21,

            Rejected = 25, // 
            Completed = 26,
            OrderPlacing = 27,

        }
        public enum ExternalOrderItemStatus
        {
            OrderPlaced = 1,
            InQc = 10,
            ReworkInProduction = 11,
            ReworkDone = 12,
            ReworkQc = 13,
            InProgress = 6,
            Delivered = 15,
            ReadyToDownload = 24,
            Rejected = 25, // 
            Completed = 26,
            ReadyToCheck = 16,
            AssignOrderToSupportTeam = 28
		}
        public enum OrderType
        { 
            NewWork=1,
            Rework=2,
            Amendment=3,
            ReworkAmendment=4,
            TestWork=5,

        }

        public enum OrderItemFileGroup
        {
            Work = 1,
            Combine = 2,
            Inside = 3,
            ColorRef = 4,
            RawDone = 5,
            Duplicate = 6
        }

        public enum ActivityLogCategory
        {
			QcUploadCompletedFileError = 1 ,
			UpdateOrderItemStatusError = 2,
			UpdateOrderError = 3,
			AddOrderStatusChangeLogError = 4,
			AddOrderItemStatusChangeLogError = 5,
			AddOrderItemError = 6,

            EditorUploadCompletedFileError=7,

			DeleteImagesOnPreviewError=8,
			GetInternalOrderStatusError=9,
            OrderUploadError=10,
            OrderListError=11,
			SingleDownloadError = 12,
			SingleDownloadCompletedFileError= 13,
			EditorRejectedFileDownloadError=14,
			SingleDownloadForClientError= 15,
			SingleDownloadEditorError = 16,
			SingleDownloadQCError = 17,
			UpdateOrderItemByQcError=18,
			RejectError= 19,
            ReplaceError=20,
			UploadReplaceFilesError=21,
			UpdateOrderItemStatusFromImagePreviewPopUpError=22,
			LoadOrderFileFolderError = 23,
			PathSplitError = 24,
			SelectOrderItemNodesFromFolderStructureError=25,
			InsertAssingOrderItemToTeamError=26,
			AssignToEditorError = 27,
			ShowAssignOrderItemsToEditorError=28,
			GetTeamIdForOrderToLoadTeamMembersError = 29,
			ShowAssignItemToTeamPopupError = 30,
			LoadTeamsForAssignOrderToSupportTeamError=31,
			InsertAssingOrderToEditorError=32,

            AutoOrderUpload = 33,

            FtpOrderPlaceApp=34,
            

            //IBR
            IbrProcessingApi = 35,
			IbrlogginApi = 36,

			NotifyOpsOnImageArrivalFTP = 37,
            OrderDeliveryToClient = 38,
            UploadFromEditorPc = 39,
            FileMoveOnSFTP = 40,
            ProgramCSError = 41,
            SFtpOrderPlaceApp = 42,

            GeneralException = 100,

            ConsoleAppStart = 110,
            ConsoleAppAppEnd = 111,
            QcFileUploadingOnKTMStorageTimeError = 120,
        }

		public enum ActivityLogType
		{
			Info = 1,
            Warning = 2,
            Error = 3,
		}
        public enum ActionType
		{
			Login = 1,
            Logout = 2,
		}
        public enum AutomatedAppEnum
        {
            ContactId = 1072
        }
        public enum ActivityLogCategoryConsoleApp
        {
            AutoCompleted = 1,
            AutoQCPass=2,
            AutoAssignToEditor=3,
            AutoAssignToTeam=4,
            AutoDownloadToEditorPc = 5,
        }
        public enum FtpOrderProcessedApp
        {
            APP1 = 1,
            APP2 = 2,
            APP3 = 3,
            APP4 = 4,
            APP5 = 5,
            APP6 = 6,
            APP7 = 7,
            APP8 = 8,
            APP9 = 9,
            APP10 = 10,
            APP11 = 11,
            APP12 = 12,
            APP13 = 13,
            APP14 = 14,
            APP15 = 15,

        }
        public enum RetouchedAiProcessStatus
        {
            Success = 1,
            Failed = 2,
            Processing=3,
        }
        public enum InputProtocolTypeEnum
		{
			FTP = 1,
			SFTP = 2,
		}
        public enum OutputProtocolTypeEnum
		{
			FTP = 1,
			SFTP = 2,
            DropBox=3,
		}
		public enum PathReplacementType
		{
			RemoveOrderNo = 1,
			RemoveOrderDate = 2,
			ReplacePath = 3,
            CategoryPath = 4,
            RemoveString = 5,
            TakeFacilityNameFromPath = 6,
            AddingFacilityNameToPath = 7,
            SubstractDuplicateFacilityNameFromPath = 8,
		}

        //its for delivery type
        public enum DeliveryType
        {
            FileToFile =1,
            ZipToZip = 2,
            ZipToFile = 3,
            FileToZip =4,

        } 
        public enum OrderPlaceBatchMoveType
		{
            FileandFolderMoveAfterOrderPlace = 1,
			FileandFolderNotMoveAfterOrderPlace = 2,
			NoActionNeeded = 3,
        }
        public enum ItemCategorySetStatus
        {

            Not_set= 1,
            Auto_set = 2,
            Manual_set = 3,
            Approved = 4,
            //Lock = 5
        }

        public enum OrderCategorySetStatus
        {

            Not_set = 1,
            Auto_set = 2,
            Manual_set = 3,
            Approved = 4,
            //Lock = 5
        }
        public enum ItemCategoryFilterType
        {
            Contains = 1,
            Equals=2,
            StartWith = 3,
            EndWith = 4,
        }
        public enum ItemCategoryLabelType
        {
            Label_1 =1,
            Label_2 =2,
            Label_3 =3,
            Label_4 =4,
            Label_5 =5,
            Label_6 =6,
            Label_7 =7,
            Label_8 =8,
            Label_9 =9,
            Label_10 =10,
            SetDefault = 100,// NO condition check
            CheckOnFullPathWithFileName = 101,
            CheckOnOnlyFileNameWithExtension = 102,
            CheckOnOnlyFileNameWithoutExtension = 103,
            CheckOnFullPathWithoutFileName = 104
        }
        /// <summary>
        /// Specifies the mode of writing a file in FTP operations.
        /// </summary>
        public enum FtpFileWriteMode
        {
            /// <summary>
            /// Writes the file in its normal format without compression.
            /// </summary>
            Normal = 1,

            /// <summary>
            /// Compresses the file into a ZIP format before uploading.
            /// </summary>
            Zip = 2
        }

        /// <summary>
        /// Specifies the type of encryption to be used for FTP connections.
        /// </summary>
        public enum FtpEncryptionModeType
        {
            /// <summary>
            /// No encryption is used (Plain text FTP).
            /// </summary>
            None = 0,

            /// <summary>
            /// FTPS encryption is used from the start of the connection, typically on port 990.
            /// </summary>
            Implicit = 1,

            /// <summary>
            /// The connection starts in plain text, and FTPS encryption is enabled with the AUTH command immediately after the server greeting.
            /// </summary>
            Explicit = 2,

            /// <summary>
            /// FTPS encryption is used if supported by the server; otherwise, it falls back to plaintext FTP communication.
            /// </summary>
            Auto = 3
        }

    }
}
