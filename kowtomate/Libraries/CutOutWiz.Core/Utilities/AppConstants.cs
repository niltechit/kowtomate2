
using static System.Net.WebRequestMethods;

namespace CutOutWiz.Core.Utilities
{
    public static class PermissionConstants
    {
        //Grid view Template and Filter
        public const string GridTemplate_AddPublicViewTemplate = "GT.PVT.A";
        public const string GridTemplate_EditPublicViewTemplate = "GT.PVT.E";

        public const string GridFilter_AddPublicFitler = "GF.PF.A";
        public const string GridFilter_EditPublicFitler = "GF.PF.E";
        public const string Report_CompareReport = "O.SA.R";

        //Notification
        public const string OrderNewOrderEmailNotifyForOPeration = "Order.ReceiveNewOrderEmailNotification";
        public const string OrderDeleteOrderEmailNotifyForOPeration = "Order.ReceiveDeleteOrderEmailNotification";
        public const string OrderUpdateOrderEmailNotifyForOPeration = "Order.ReceiveUpdateOrderEmailNotification";

        //Order Access Permission
        public const string Order_ViewAllClientOrders = "Order.ViewAllOrders";
        public const string Order_ViewAllTeamOrders = "Order.ViewAllTeamOrders";
        public const string Order_ViewAllQCOrders = "Order.ViewAllQCOrders";
		public const string Order_ViewAllAssignedOrders = "Order.ViewAllAssignedOrders";
        public const string Order_ViewAllAssignedOrderItem = "Order.ViewAllAssignedOrderItem";
        public const string Order_ViewAllProductionDoneOrder = "Order.ViewAllProductionDoneOrder";
        public const string Order_ViewAllAssignedOrderItemByAnotherTeamLead = "Order.ViewAllAssignedOrderItemByAnotherTeamLead";
        public const string Order_QCUploadCompletedFiles = "Order.QCUploadCompletedFiles";
        public const string CanViewExternalStatus = "Order.CanViewExternalStatus";
        public const string CanViewInternalStatus = "Order.CanViewInternalStatus";
        public const string OrderItem_CanApprovedByQC = "Order.CanApproveByQc";
        public const string OrderItem_CanRejectByQC = "Order.CanRejectByQc";
        public const string Order_UpdateOrderForClientDownload = "Order.UpdateOrderForClientDownload";
        public const string Order_CanViewStatusWiseDownloadButton = "Order_CanViewStatusWiseDownloadButton";

        //Price Confimation Permission
        public const string CanEditPriceByOps = "SOP.CanEditPriceByOps";
        //public const string CanEditPriceByClient = "SOP.CanEditPriceByClient";
        public const string SOP_CanViewPrice = "SOP.CanViewPrice";

        //ShowOrderColumnPermisson
        public const string AssignNewOrderToTeam = "Order.AssignNewOrderToTeam";
        public const string AssignNewOrderToEditor = "Order.AssignNewOrderToEditor";

        public const string AssignNewOrderItemToTeam = "Order.AssignNewOrderItemToTeam";


        //ShowAllTeamMembersPermission
        public const string CanViewAllTeamMembers = "Team.CanViewAllTeamMembers";

        //Client companies Permissions
        public const string SendMailCompanyCreateForOPeration = "company.CreateNotificationForOPerations";
        //public const string SendMailCompanyCreateForOPeration = "EmailNotification.NewCompanyCreateNotificationForAdmin";

        //CanUpdateOrderStatusAsEditor
        public const string Order_UpdateOrderAllItemStatusInProduction = "Order.CanUpdateOrderAllItemStatusInProduction";
        public const string Order_UpdateOrderAllItemStatusInQc = "Order.CanUpdateOrderAllItemStatusInQc";

        // Order details Page Staus Log button Colum
        public const string Order_CanViewOrderItemStatusLog = "Order.CanViewOrderItemStatusLog";

        //Order Item Leave and seize
        public const string Order_CanSeizeOrderItem = "Order.CanSeizeOrderItem";
        public const string Order_CanLeaveOrderItem = "Order.CanLeaveOrderItem";
        public const string CanViewOperationDashboard = "Dashboard.CanViewOperationDashboard";
		// Order Item Re Assign 
		public const string Order_item_reassign_for_qc = "Order.QCReassignFiles";
        //Order Item Download By Client

        //team status access permission
        public const string Order_CanViewTeamStatus = "Order.CanViewTeamStatus";

        //public const string Security_User_Password_Change_For_Admin = "Security.CanViewPreviousPasswordField";
        public const string Security_UserPasswordChangeForAdmin = "Security.CanEmployeePasswordChanges";
        public const string HR_EmployeeLeaveApproved = "HR.CanApprovedLeave";
        public const string HR_ViewAllLeaves = "HR.CanViewAllEmployeeLeaves";
        public const string HR_ViewAllEmployeeprofiles = "HR.CanViewAllEmployeeProfiles";
        public const string HR_ViewAllSubLeaveType = "HR.CanViewAllSubLeaveType";
        public const string HR_ViewAllLeaveType = "HR.CanViewAllLeaveType";
        public const string HR_ManageDesignations = "HR.ManageDesignations";
        public const string HR_CanAddEmployeeProfile = "HR.CanAddEmployeeProfile";
        public const string HR_ViewEmployeeLeave = "HR.CanViewEmployeeLeave";
        public const string HR_ApprovedButtonShow = "HR.CanShowApprovedButtonForApproved";
        
        public const string Accouting_ViewAllOverheadCost = "accounting.CanViewOverheadCostList";
        public const string Accouting_CanAddNewOverheadCost = "accounting.CanAddNewOverheadCost";
        public const string Category_Canviewallcategoryservicelist = "Category.CanViewClientCategoryServicesList";
        public const string Category_CanViewAllClientCategoryList = "Category.CanViewClientCategoryList";
        public const string Category_CanViewAllCommonCategoryList = "Category.CanViewCommonCategoryList";
        public const string Category_CanAddCategoryPrice = "Category.ClientCategoryPriceCanAdd";
        public const string Category_CanViewAllCommonServiceList = "Category.CanViewCommonServicesList";
        public const string Category_CanViewClientCategoryPrice = "Category.ClientCategoryPriceCanSee";
	}

    public static class StatusConstants
    {
        public const int Active = 1;
        public const int InActive = 2;
        public const int Deleted = 3;
    }

    public static class ApprovalToolActionTypeConstants
    {
        public const string Accepted = "Accepted";
        public const string Rejected = "Rejected";
    }

    public static class ApprovalToolImageFoldersConstants
    {
        public const string ToProcess = "To Process";
        public const string ForApproval = "For Approval";
        public const string ForRework = "For Rework";
        public const string Completed = "Completed";
    }

    public static class ClaimTypesConstants
    {
        public const string UserId = "UserId";
        public const string ContactId = "ContactId";
        public const string CompanyId = "CompanyId";
        public const string CompanyObjectId = "CompanyObjectId";
        //public const string SuperAdmin = "SuperAdmin";
        public const string ClientRootFolderPath = "ClientRootFolderPath";
        public const string CompanyType = "CompanyType";
        public const string FullName = "FullName";
        public const string RoleObjectId = "RoleObjectId";
		public const string TeamId = "TeamId";
		public const string Username = "Username";
	}

    public static class SourceDriveConstants
    {
        public const string AmazonS3 = "Amazon S3";
    }

    public static class ActivityLogForConstants
    {
        public const byte SOPTemplate = 1;
        public const byte Order = 2;
        public const byte OrderItem = 3;
        public const byte OrderItemDownloadToEditorPcByAutomatedUser = 4;
        public const byte Company = 5;
        public const byte ConsoleAppId = 6;
        public const byte GeneralLog = 100;
    }

    public static class CompanyCodeConstants
    {
        public const string AdminCompany = "KTMSYS";
        public const string BaseClientCompany = "KTMBCL";
    }

    public enum EmailQueueStatus
    {
        Any = 0,
        Sent = 1,
        NotSent = 2,
        Failed = 0,
        Retry = 3,
        Deleted = 4
    }

    public static class FileStatusWiseLocationOnFtpConstants
    {
        public const string Raw = "Raw";
        public const string InProgress = "In Progress";
        public const string Completed = "Completed";
    }

    public static class AutomatedAppConstant
    {
        //public const int ContactId = 1270; // Test Automated User
        //public const int VcTeamId = 8; //Test
        //public const int SupportingEditorContactId = 1097; //Test
        //public const double minPerImage = 1.33; //Test
        //public const string timeTrackerUrl = "C:\\Kowtomate(Test) Time Tracker";
        //public const string ftprootPath = "/KOW_NAS_STORAGE/KOW_ROOT_FOLDER/KowToMate_Software/";
        //public const int VcCompanyId = 1193;

        //         Must Give on Production 
        public const int ContactId = 1322; // Production Automated User
        public const int VcTeamId = 9; // Production
        public const int VcCompanyId = 1181; // Production
        public const int SupportingEditorContactId = 1323; //Production
        public const double minPerImage = .8;
        public const string timeTrackerUrl = "C:\\Kowtomate(Live) Time Tracker";

        public const string DefaultOrderPlacedFileContainer = "_downloaded";

		public const string MoveFileAsCompleted = "Auto Completed";
     
        public const double VcDeadLineInHour = 1.5;
        public const int VcWarningTime = 20;
        public const int VcInDangerTime = 10;


        //Six : Need to take to DB
		public const double SixPriorityBatchDeadLineInHour = 12;
		public const double SixGeneralBatchDeadLineInHour = 24;
		public const int SixCompanyId = 1176;

        //IBR
        //public const string IbrWebApiBaseUrl = "http://103.197.204.22:8007/api/2023-02";
        public const string IbrWebApiBaseUrl = "https://api1.retouched.ai/api/2023-02";
        public const string IbrProcessApiBaseUrl = "http://192.168.1.219:8008/v.03.13.23";
        public const string ClientUploadCompletedIndicator = "uploadcompleted.txt";
        public const string GLS_AMSClientOrderPlaceIndicator = "Hotfolder.New";

		//GNR
		public const int GNRCompanyId = 1183;

        //MLS
        public const string extractParentFolder = "ExtractPath";

        //MNM Team id
        //public const int MNMTeamId = 8; //dev constant

		public const int MNMTeamId = 13; // production constant
		public const int JBLTeamId = 15; // production constant
		public const int SOSTeamId = 12; // production constant

        //Must Give on Production 
        public const int dangerWarningPercent = 30;
        public const int dangerPercent = 20;
    }

	public static class EmailTemplateConstants
    {
        public const string NotifyOpsOnImageArrivalFTP = "FTPOrder.NotifyOpsOnImageArrivalFTP";
		public const string NotifyOpsOnOrderPlaced = "FTPOrder.NotifyOpsOnOrderPlaced";
		public const string NotifyOpsOnOrderDeliveryToClient = "FTPOrder.NotifyOpsOnOrderDeliveryToClient";
		public const string NotifyOpsForCloudStorageUsesLimitation = "NotifyOpsForCloudStorageUsesLimitation";
		public const string NotificationOrderPlacementEmailSendToClientCompany = "NotificationOrderPlacementEmailSendToClientCompany";
	}

    public static class RoleNameConstant
    {
        public const string EditorRoleName = "Editor";
    }

    public class Gender
    {
        public const string Male = "Male";
        public const string Female = "Female";
        public const string Other = "Other";
    }

    public static class LeaveStatus
    {
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Pending = "Pending";
    }

    public static class EmployeeRoles
    {
        public const string Editor = "";
        public const string QC = "QC";
        public const string TeamLead = "TL";
    }
}
