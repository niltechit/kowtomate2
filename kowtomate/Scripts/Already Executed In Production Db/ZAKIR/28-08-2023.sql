ALTER TABLE CompanyGeneralSettings
ADD IsBatchRootFolderNameAddWithOrder BIT DEFAULT 0 

GO

ALTER PROCEDURE [dbo].[SP_CompanyGeneralSettings_Insert]
    @AutoAssignOrderToTeam bit,
    @AutoDistributeToEditor bit,
    @AutoQcPass bit,
    @AutoOperationPass bit,
    @AllowPartialDelivery bit,
    @EnableFtpOrderPlacement bit,
    @EnableOrderDeliveryToFtp bit,
    @CompanyId int,
	@AllowSingleOrderFromFTP bit,
	@AllowMaxNumOfItemsPerOrder int,
	@EnableSingleOrderPlacement bit,
	@IsIbrProcessedEnabled bit,
	@AllowAutoUploadFromEditorPc bit,
	@AllowAutoUploadFromQcPc bit,
	@AllowAutoDownloadToEditorPc bit,
	@AllowClientWiseImageProcessing bit,
	@AllowNotifyOpsOnImageArrivalFTP bit,
	@CheckUploadCompletedFlagOnFile bit,
	@CheckEmailForUploadCompletedConfirmation bit,
	@FtpOrderPlacedAppNo smallint,
	@IsBatchRootFolderNameAddWithOrder bit
AS
BEGIN
    DECLARE @Id int
    
    INSERT INTO CompanyGeneralSettings
        (
			AutoAssignOrderToTeam, 
			AutoDistributeToEditor, 
			AutoQcPass, 
			AutoOperationPass, 
			AllowPartialDelivery, 
			EnableFtpOrderPlacement, 
			EnableOrderDeliveryToFtp, 
			CompanyId,
			AllowSingleOrderFromFTP,
			EnableSingleOrderPlacement,
			AllowMaxNumOfItemsPerOrder,
			IsIbrProcessedEnabled,
			AllowAutoUploadFromEditorPc,
			AllowAutoUploadFromQcPc,
			AllowAutoDownloadToEditorPc,
			AllowClientWiseImageProcessing,
			AllowNotifyOpsOnImageArrivalFTP,
			CheckUploadCompletedFlagOnFile,
			CheckEmailForUploadCompletedConfirmation,
			FtpOrderPlacedAppNo,
			IsBatchRootFolderNameAddWithOrder
		)
    VALUES
        (
			@AutoAssignOrderToTeam, 
			@AutoDistributeToEditor, 
			@AutoQcPass, 
			@AutoOperationPass, 
			@AllowPartialDelivery, 
			@EnableFtpOrderPlacement, 
			@EnableOrderDeliveryToFtp, 
			@CompanyId,
			@AllowSingleOrderFromFTP,
			@EnableSingleOrderPlacement,
			@AllowMaxNumOfItemsPerOrder,
			@IsIbrProcessedEnabled,
			@AllowAutoUploadFromEditorPc,
			@AllowAutoUploadFromQcPc,
			@AllowAutoDownloadToEditorPc,
			@AllowClientWiseImageProcessing,
			@AllowNotifyOpsOnImageArrivalFTP,
			@CheckUploadCompletedFlagOnFile,
			@CheckEmailForUploadCompletedConfirmation,
			@FtpOrderPlacedAppNo,
			@IsBatchRootFolderNameAddWithOrder
		)
    
    SELECT SCOPE_IDENTITY();
END

GO

ALTER PROCEDURE [dbo].[SP_CompanyGeneralSettings_Update]
    @Id int,
    @AutoAssignOrderToTeam bit = NULL,
    @AutoDistributeToEditor bit = NULL,
    @AutoQcPass bit = NULL,
    @AutoOperationPass bit = NULL,
    @AllowPartialDelivery bit = NULL,
    @EnableFtpOrderPlacement bit = NULL,
    @EnableOrderDeliveryToFtp bit = NULL,
    @CompanyId int = NULL,
	@AllowSingleOrderFromFTP bit = NULL,
	@AllowMaxNumOfItemsPerOrder int = NULL,
	@EnableSingleOrderPlacement bit = NULL,
	@IsIbrProcessedEnabled bit = NULL,
	@AllowAutoUploadFromEditorPc bit = NULL,
	@AllowAutoUploadFromQcPc bit = NULL,
	@AllowAutoDownloadToEditorPc bit = NULL,
	@AllowClientWiseImageProcessing bit = NULL,
	@AllowNotifyOpsOnImageArrivalFTP bit = NULL,
	@CheckUploadCompletedFlagOnFile bit = NULL,
	@CheckEmailForUploadCompletedConfirmation bit = NULL,
	@FtpOrderPlacedAppNo smallint,
	@IsBatchRootFolderNameAddWithOrder bit=null
AS
BEGIN
    UPDATE CompanyGeneralSettings
    SET AutoAssignOrderToTeam = ISNULL(@AutoAssignOrderToTeam, AutoAssignOrderToTeam),
        AutoDistributeToEditor = ISNULL(@AutoDistributeToEditor, AutoDistributeToEditor),
        AutoQcPass = ISNULL(@AutoQcPass, AutoQcPass),
        AutoOperationPass = ISNULL(@AutoOperationPass, AutoOperationPass),
        AllowPartialDelivery = ISNULL(@AllowPartialDelivery, AllowPartialDelivery),
        EnableFtpOrderPlacement = ISNULL(@EnableFtpOrderPlacement, EnableFtpOrderPlacement),
        EnableOrderDeliveryToFtp = ISNULL(@EnableOrderDeliveryToFtp, EnableOrderDeliveryToFtp),
        CompanyId = ISNULL(@CompanyId, CompanyId),
		AllowSingleOrderFromFTP = ISNULL(@AllowSingleOrderFromFTP, AllowSingleOrderFromFTP),
		AllowMaxNumOfItemsPerOrder = ISNULL(@AllowMaxNumOfItemsPerOrder, AllowMaxNumOfItemsPerOrder),
		EnableSingleOrderPlacement = ISNULL(@EnableSingleOrderPlacement, EnableSingleOrderPlacement),
		IsIbrProcessedEnabled = ISNULL(@IsIbrProcessedEnabled, IsIbrProcessedEnabled),
		AllowAutoUploadFromEditorPc = ISNULL(@AllowAutoUploadFromEditorPc, AllowAutoUploadFromEditorPc),
		AllowAutoUploadFromQcPc = ISNULL(@AllowAutoUploadFromQcPc, AllowAutoUploadFromQcPc),
		AllowAutoDownloadToEditorPc = ISNULL(@AllowAutoDownloadToEditorPc, AllowAutoDownloadToEditorPc),
		AllowClientWiseImageProcessing = ISNULL(@AllowClientWiseImageProcessing, AllowClientWiseImageProcessing),
		AllowNotifyOpsOnImageArrivalFTP = ISNULL(@AllowNotifyOpsOnImageArrivalFTP, AllowNotifyOpsOnImageArrivalFTP),
		CheckUploadCompletedFlagOnFile = ISNULL(@CheckUploadCompletedFlagOnFile, CheckUploadCompletedFlagOnFile),
		CheckEmailForUploadCompletedConfirmation = ISNULL(@CheckEmailForUploadCompletedConfirmation, CheckEmailForUploadCompletedConfirmation),
		FtpOrderPlacedAppNo = ISNULL(@FtpOrderPlacedAppNo, FtpOrderPlacedAppNo),
		IsBatchRootFolderNameAddWithOrder = ISNULL(@IsBatchRootFolderNameAddWithOrder, IsBatchRootFolderNameAddWithOrder)
    WHERE Id = @Id
END
