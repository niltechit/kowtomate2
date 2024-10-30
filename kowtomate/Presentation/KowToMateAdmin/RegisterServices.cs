#region Using
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Services.HR;
using CutOutWiz.Services.Email;
using CutOutWiz.Services.Management;
using CutOutWiz.Services.Security;
using CutOutWiz.Services.SOP;
using CutOutWiz.Services.EmailMessage;
using CutOutWiz.Services.EmailSender;
using CutOutWiz.Services.LogServices;
using Radzen;
using CutOutWiz.Services.Logs;
using CutOutWiz.Services.Log;
using CutOutWiz.Services.StorageService;
using CutOutWiz.Services.ClientOrders;
using CutOutWiz.Services.InternalMessage;
using CutOutWiz.Services.FolderServices;
using CutOutWiz.Services.OrderTeamServices;
using CutOutWiz.Services.MessageService;
using CutOutWiz.Services.OrderAndOrderItemStatusChangeLogServices;
using CutOutWiz.Services.OrderItemStatusChangeLogService;
using CutOutWiz.Services.OrderSOP;
using CutOutWiz.Services.Dashboards;
using KowToMateAdmin.Services;
using CutOutWiz.Services.BLL.OrderStatusAndOrderItemStatus;
using CutOutWiz.Services.BLL.UpdateOrderItem;
using CutOutWiz.Services.BLL.StatusChangeLog;
using CutOutWiz.Services.BLL;
using CutOutWiz.Services.ReportServices;
using CutOutWiz.Services.DynamicReports;
using CutOutWiz.Services.PathReplacementServices;
using CutOutWiz.Services.BLL.OrderAttachment;
using CutOutWiz.Services.CpanelStorageInfoServices;
using CutOutWiz.Services.FeedbackReworkServices;
using CutOutWiz.Services.ClientCommonCategoryService.CommonCategories;
using CutOutWiz.Services.EncryptedMethodServices;
using CutOutWiz.Services.Managers.Accounting;
using CutOutWiz.Services.AutomationAppServices.FtpOrderProcess;
using CutOutWiz.Services.AutomationAppServices.MakeOrderPlacingToPlaced;
using CutOutWiz.Services.ErrorLogServices;
using CutOutWiz.Services.AutomationAppServices.UploadFromEditorPcAutomation;
using CutOutWiz.Services.AutomationAppServices.UploadFromQcPcAutomation;
using CutOutWiz.Services.AutomationAppServices.OrderPlaceAutomation;
using CutOutWiz.Services.AutomationAppServices.OrderDeliveryAutomation;
using CutOutWiz.Services.AutomationAppServices.DownloadToEditorAutomation;
using CutOutWiz.Services.AutomationAppServices.OrderWorkFlowAutomationServices;
using CutOutWiz.Services.AutomationAppServices.EmailSendAutomation;
using CutOutWiz.Services.FtpFileDeleteServices;
using CutOutWiz.Services.IbrApiServices;
using CutOutWiz.Services.BLL.AssignOrderAndItem;
using CutOutWiz.Services.AutomationAppServices.DeleteFilesFromNasAutomation;
using CutOutWiz.Services.ClaimManagementService;
using CutOutWiz.Services.WinSCPFtpService;
using CutOutWiz.Services.MapperHelper;
using CutOutWiz.Services.AutomationAppServices.OrderItemCategorySetByAutomation;
using CutOutWiz.Services.UI;
using CutOutWiz.Services.ImportExport;
using CutOutWiz.Services.AutomationAppServices.ConvertOrderAttachmentFiles;
using CutOutWiz.Services.Helper;
using CutOutWiz.Services.OperationSwitchService;
using CutOutWiz.Services.SftpServices;
using CutOutWiz.Services.WebApiService;
using CutOutWiz.Services.ClientCommonCategoryService.ClientCategorys;
using CutOutWiz.Services.ClientCommonCategoryService.CommonCategoryServices;
using CutOutWiz.Services.ClientCommonCategoryService.ClientCategoryServices;
using CutOutWiz.Data.Repositories.Security;
using CutOutWiz.Data.Repositories.Common;
using CutOutWiz.Services.Managers.Common;
using CutOutWiz.Data.Repositories.Accounting;

#endregion end of Using

namespace KowToMateAdmin
{
    public static class RegisterServices
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<DialogService>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<IWorkContext, WorkContext>();
            builder.Services.AddTransient<ICustomAuthenticationService, CustomAuthenticationService>();
            builder.Services.AddTransient<ISqlDataAccess, SqlDataAccess>();

            //Repositories
            //Remove this after all convertion
            builder.Services.AddTransient<CutOutWiz.Data.DbAccess.ISqlDataAccess, CutOutWiz.Data.DbAccess.SqlDataAccess>();

            builder.Services.AddTransient<IRoleRepository, RoleRepository>();
			builder.Services.AddTransient<IContactRepository, ContactRepository>();
            builder.Services.AddTransient<ICompanyRepository, CompanyRepository>();
            builder.Services.AddTransient<ICompanyTeamRepository, CompanyTeamRepository>();
            builder.Services.AddTransient<ICountryRepository, CountryRepository>();
            builder.Services.AddTransient<IFileServerRepository, FileServerRepository>();
            builder.Services.AddTransient<IShiftRepository, ShiftRepository>();
            builder.Services.AddTransient<ICompanyGeneralSettingRepository, CompanyGeneralSettingRepository>();
            builder.Services.AddTransient<IOverheadCostRepository, OverheadCostRepository>();

			//End of Repositories

			builder.Services.AddTransient<IDesignationService, DesignationService>();
            builder.Services.AddTransient<ICompanyManager, CompanyManager>();
            builder.Services.AddTransient<ICountryManager, CountryManager>();
            builder.Services.AddTransient<IContactManager, ContactManager>();
			//builder.Services.AddTransient<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
			builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IModuleService, ModuleService>();
            builder.Services.AddTransient<IMenuService, MenuService>();
            builder.Services.AddTransient<IPermissionService, PermissionService>();
            builder.Services.AddTransient<IRoleManager, RoleManager>();
            builder.Services.AddTransient<IFileServerManager, FileServerManager>();
            builder.Services.AddTransient<IEmailSenderAccountService, EmailSenderAccountService>();
            builder.Services.AddTransient<IEmailTemplateService, EmailTemplateService>();
            builder.Services.AddTransient<ITeamService, TeamService>();
            builder.Services.AddTransient<ITeamRoleService, TeamRoleService>();
            builder.Services.AddTransient<ITeamMemberService, TeamMemberService>();
            builder.Services.AddTransient<IOrderSOPStandardServiceService, OrderSOPStandardServiceService>();
            builder.Services.AddTransient<IOrderSOPTemplateService, OrderSOPTemplateService>();

            builder.Services.AddTransient<ILogService, LogService>();
            builder.Services.AddTransient<IWorkflowEmailService, WorkflowEmailService>();
            builder.Services.AddTransient<IEmailTokenProvider, EmailTokenProvider>();
            builder.Services.AddTransient<IEmailTokenizer, EmailTokenizer>();
            builder.Services.AddTransient<IMailjetEmailService, MailjetEmailService>();
            builder.Services.AddTransient<ILogServices,LogServices>();
            builder.Services.AddTransient<IGCPService,GCPService>();
            builder.Services.AddTransient<IFtpService,FtpService>();
            builder.Services.AddTransient<IOrderTemplateService, OrderTemplateService>();
            builder.Services.AddTransient<IOrderSOPTempleateFileService, OrderSOPTempleateFileService>();

            builder.Services.AddTransient<IClientOrderItemService, ClientOrderItemService>();
            builder.Services.AddTransient<IClientOrderService, ClientOrderService>();
            builder.Services.AddTransient<IOrderFileAttachmentService, OrderFileAttachmentService>();
            builder.Services.AddTransient<IOperationEmailService, OperationEmailService>();

            builder.Services.AddTransient<IArchiveQueueEmailService, ArchiveQueueEmailService>();
            builder.Services.AddTransient<IInternalMessageTemplateService, InternalMessageTemplateService>();
            builder.Services.AddTransient<IInternalMessageService, InternalMessageService>();
            builder.Services.AddTransient<ILocalFileService, LocalFileService>();
            builder.Services.AddTransient<IFolderService, FolderService>();
            builder.Services.AddTransient<IInternalMessageTokenProvider, InternalMessageTokenProvider>();
            builder.Services.AddTransient<IInernalMessageTokenizer, InernalMessageTokenizer>();
            builder.Services.AddTransient<IOrderTeamService, OrderTeamService>();
            builder.Services.AddTransient<IOrderAssignedImageEditorService, OrderAssignedImageEditorService>();
            builder.Services.AddTransient<ICompanyTeamManager, CompanyTeamManager>();
            builder.Services.AddTransient<IOrderFileAssignService, OrderFileAssignService>();
            builder.Services.AddTransient<IOrderItemStatusChangeLogService, OrderItemStatusChangeLogService>();
            builder.Services.AddTransient<IOrderStatusChangeLogService, OrderStatusChangeLogService>();
            builder.Services.AddScoped<NotificationService>();
            //builder.Services.AddTransient<IPageImage, PageImage>();

            builder.Services.AddTransient<IDownloadService,DownloadService>();
            builder.Services.AddTransient<IFluentFtpService, FluentFtpService>();

            builder.Services.AddTransient<IOrderTemplateSOPService, OrderTemplateSOPService>();
            builder.Services.AddTransient<IOrderSOPTemplateOrderSOPStandardService, OrderSOPTemplateOrderSOPStandardService>();
            builder.Services.AddTransient<IOrderSOPStandardService, OrderSOPStandardService>();
            builder.Services.AddTransient<IOrderTempleateSOPFileService, OrderTempleateSOPFileService>();
            builder.Services.AddTransient<IFtpFilePathService, FtpFilePathService>();
            builder.Services.AddTransient<IOrderSOPTemplateJoiningService, OrderSOPTemplateJoiningService>();
            builder.Services.AddTransient<IDashboardService, DashboardService>();
            builder.Services.AddTransient<IClientOrderFtpService, ClientOrderFtpService>();
            builder.Services.AddTransient<ICompanyGeneralSettingManager, CompanyGeneralSettingManager>();
            
            builder.Services.AddServerSideBlazor().AddCircuitOptions(option => { option.DetailedErrors = true; });

            builder.Services.AddTransient<IOrderStatusService, OrderStatusService>();
			builder.Services.AddTransient<IUpdateOrderItemBLLService, UpdateOrderItemBLLService>();
			builder.Services.AddTransient<IPathReplacementService, PathReplacementService>();

			//builder.Services.AddServerSideBlazor().AddCircuitOptions(option => { option.DetailedErrors = true; });

			builder.Services.AddTransient<IAcitivityLogCommonMethodService, AcitivityLogCommonMethodService>();
			builder.Services.AddTransient<IStatusChangeLogBLLService, StatusChangeLogBLLService>();
			builder.Services.AddTransient<IActivityAppLogService, ActivityAppLogService>();
            builder.Services.AddTransient<IShiftManager, ShiftManager>();

            //Dynamic reports
            builder.Services.AddTransient<IDynamicReportInfoService, DynamicReportInfoService>();
            builder.Services.AddTransient<IGridViewSetupService, GridViewSetupService>();
            //builder.Services.AddTransient<IDataImportService, DataImportService>();
            builder.Services.AddTransient<IDataImportExportService, DataImportExportService>();
            builder.Services.AddTransient<IGridFilterService, GridFilterService>();
            builder.Services.AddTransient<IDynamicReportPageViewFilterService, DynamicReportPageViewFilterService>();

            builder.Services.AddTransient<IOperationReportService, OperationReportService>();
			builder.Services.AddTransient<IAutoOrderDeliveryService,AutoOrderDeliveryService>();
			builder.Services.AddTransient<ISecurityLoginHistoryService, SecurityLoginHistoryService>();
			builder.Services.AddTransient<ISshNetService, SshNetService>();
			builder.Services.AddTransient<IOrderAttachmentBLLService, OrderAttachmentBLLService>();
			builder.Services.AddTransient<IFluentFtpService, FluentFtpService>();
			builder.Services.AddTransient<ICpanelStorageInfoService, CpanelStorageInfoService>();
			builder.Services.AddTransient<IFeedbackReworkService, FeedbackReworkService>();
            // Client Category Created Services
			builder.Services.AddTransient<IClientCategoryService, ClientCategoryService>();
			builder.Services.AddTransient<ICommonServiceService, CommonServiceService>();
			builder.Services.AddTransient<ICommonCategoryServiceService, CommonCategoryServiceService>();
			builder.Services.AddTransient<ICommonCategoryService, CommonCategoryService>();
			builder.Services.AddTransient<IClientCategoryServiceService, ClientCategoryServiceService>();
			builder.Services.AddTransient<IEncryptedMethodService, EncryptedMethodService>();
            builder.Services.AddTransient<IManageTeamMemberChangelogService,ManageTeamMemberChangelogService>();
            // Overhead cost
            builder.Services.AddTransient<IOverheadCostManager, OverheadCostManager>();
            builder.Services.AddTransient<IFileConvertionService, FileConvertionService>();

            builder.Services.AddTransient<IOrderPlacingToPlacedService, OrderPlacingToPlacedService>();
            builder.Services.AddTransient<IOrderPlaceService, OrderPlaceService>();
            builder.Services.AddTransient<IErrorLogService, ErrorLogService>();
            builder.Services.AddTransient<IUploadFromEditorPcService, UploadFromEditorPcService>();
            builder.Services.AddTransient<IUploadFromQcPcService, UploadFromQcPcService>();
            builder.Services.AddTransient<IOrderDeliveryService, OrderDeliveryService>();
            builder.Services.AddTransient<IDownloadToEditorService, DownloadToEditorService>();
            builder.Services.AddTransient<IOrderWorkFlowAutomationService, OrderWorkFlowAutomationService>();
            builder.Services.AddTransient<IAutoEmailSendService, AutoEmailSendService>();

            builder.Services.AddTransient<IFtpFileDeleteService, FtpFileDeleteService>();
            builder.Services.AddTransient<IClientExternalOrderFTPSetupService, ClientExternalOrderFTPSetupService>();
            builder.Services.AddTransient<IIbrApiService, IbrApiService>();
            builder.Services.AddTransient<IAssingOrderItemService, AssingOrderItemService>();
            builder.Services.AddTransient<IDeleteFilesFromNasService, DeleteFilesFromNasService>();
            builder.Services.AddTransient<IClaimsService,ClaimsService>();

            //Reporting            
            builder.Services.AddTransient<ICompletedFilesComparisionReportService, CompletedFilesComparisionReportService>();
            builder.Services.AddTransient<IWinSCPFtpLibraryService,WinSCPFtpLibraryService>();
            builder.Services.AddTransient<ILeaveTypeService, LeaveTypeService>();
            builder.Services.AddTransient<IEmployeeProfileService, EmployeeProfileService>();
            builder.Services.AddTransient<IEmployeeLeaveService, EmployeeLeaveService>();
            builder.Services.AddTransient<ILeaveSubTypeService, LeaveSubTypeService>();
            builder.Services.AddTransient<IMapperHelperService, MapperHelperService>();
            builder.Services.AddTransient<IClientCategoryBaseRuleService, ClientCategoryBaseRuleService>();
            builder.Services.AddTransient<ICategorySetService,CategorySetService>();
            builder.Services.AddTransient<IConvertOrderAttachmentFile, ConvertOrderAttachmentFile>();
            builder.Services.AddTransient<IPDFConverstionService, PDFConverstionService>();
            builder.Services.AddTransient<ISwitchOperation, SwitchOperation>();
            builder.Services.AddTransient<ISftpService, SftpService>();
            builder.Services.AddTransient<IFtpSharpLibraryService, FtpSharpLibraryService>();
            builder.Services.AddTransient<IFileViewApiService, FileViewApiService>();
            builder.Services.AddTransient<CloudStorageUsesLimitAlertBackgroundService>();
            builder.Services.AddHostedService(provider => provider.GetRequiredService<CloudStorageUsesLimitAlertBackgroundService>());


        }
    }

}
