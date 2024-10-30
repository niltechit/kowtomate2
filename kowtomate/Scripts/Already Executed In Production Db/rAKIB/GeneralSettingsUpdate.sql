  alter table dbo.CompanyGeneralSettings add CheckUploadCompletedFlagOnBatchName bit
  alter table dbo.CompanyGeneralSettings add CompletedFlagKeyNameOnBatch nvarchar(200) 





GO
/****** Object:  StoredProcedure [dbo].[SP_CompanyGeneralSettings_Insert]    Script Date: 11/23/2023 12:49:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
	@IsBatchRootFolderNameAddWithOrder bit,
	@AllowExtraFile bit,
	@isFtpFolderPreviousStructureWiseStayInFtp bit,
	@IsSameNameImageExistOnSameFolder bit,
	@DeliveryType smallint,
	@OrderPlaceBatchMoveType smallint,
	@RemoveFacilityNameFromOutputRootFolderPath bit ,
	@CheckUploadCompletedFlagOnBatchName bit,
	@CompletedFlagKeyNameOnBatch nvarchar(200)
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
			IsBatchRootFolderNameAddWithOrder,
			AllowExtraFile,
			isFtpFolderPreviousStructureWiseStayInFtp,
			IsSameNameImageExistOnSameFolder,
			DeliveryType,
			OrderPlaceBatchMoveType,
			RemoveFacilityNameFromOutputRootFolderPath,
			CheckUploadCompletedFlagOnBatchName,
			CompletedFlagKeyNameOnBatch
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
			@IsBatchRootFolderNameAddWithOrder,
			@AllowExtraFile,
			@isFtpFolderPreviousStructureWiseStayInFtp,
			@IsSameNameImageExistOnSameFolder,
			@DeliveryType,
			@OrderPlaceBatchMoveType,
			@RemoveFacilityNameFromOutputRootFolderPath,
			@CheckUploadCompletedFlagOnBatchName,
			@CompletedFlagKeyNameOnBatch
		)
    
    SELECT SCOPE_IDENTITY();
END


GO
/****** Object:  StoredProcedure [dbo].[SP_CompanyGeneralSettings_Update]    Script Date: 11/23/2023 12:53:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
	@IsBatchRootFolderNameAddWithOrder bit=null,
	@AllowExtraFile bit=NULL,
	@isFtpFolderPreviousStructureWiseStayInFtp bit=NULL,
	@IsSameNameImageExistOnSameFolder bit = NULL,
	@DeliveryType smallint = null,
	@OrderPlaceBatchMoveType smallint = null,
	@RemoveFacilityNameFromOutputRootFolderPath bit =null,
	@CheckUploadCompletedFlagOnBatchName bit = null,
	@CompletedFlagKeyNameOnBatch nvarchar(200) = null
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
		IsBatchRootFolderNameAddWithOrder = ISNULL(@IsBatchRootFolderNameAddWithOrder, IsBatchRootFolderNameAddWithOrder),
		AllowExtraFile = ISNULL(@AllowExtraFile, AllowExtraFile),
		isFtpFolderPreviousStructureWiseStayInFtp = ISNULL(@isFtpFolderPreviousStructureWiseStayInFtp, isFtpFolderPreviousStructureWiseStayInFtp),
		IsSameNameImageExistOnSameFolder  = ISNULL(@IsSameNameImageExistOnSameFolder, IsSameNameImageExistOnSameFolder),
		DeliveryType  = ISNULL(@DeliveryType, DeliveryType),
		OrderPlaceBatchMoveType  = ISNULL(@OrderPlaceBatchMoveType, OrderPlaceBatchMoveType),
		RemoveFacilityNameFromOutputRootFolderPath  = ISNULL(@RemoveFacilityNameFromOutputRootFolderPath, RemoveFacilityNameFromOutputRootFolderPath),
		CheckUploadCompletedFlagOnBatchName  = ISNULL(@CheckUploadCompletedFlagOnBatchName, CheckUploadCompletedFlagOnBatchName),
		CompletedFlagKeyNameOnBatch  = ISNULL(@CompletedFlagKeyNameOnBatch, CompletedFlagKeyNameOnBatch)
    WHERE Id = @Id
END