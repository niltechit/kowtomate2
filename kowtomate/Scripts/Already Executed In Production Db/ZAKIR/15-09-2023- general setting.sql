USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_CompanyGeneralSettings_Insert]    Script Date: 9/15/2023 2:48:55 PM ******/
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
	@isFtpFolderPreviousStructureWiseStayInFtp bit,
	@AllowExtraFile bit
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
			isFtpFolderPreviousStructureWiseStayInFtp,
			AllowExtraFile
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
			@isFtpFolderPreviousStructureWiseStayInFtp,
			@AllowExtraFile
		)
    
    SELECT SCOPE_IDENTITY();
END