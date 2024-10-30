
ALTER TABLE CompanyGeneralSettings
ADD OrderPlaceBatchMoveType SMALLiNT;

alter table CompanyGeneralSettings
add RomveFacilityNameFromOutputRootFolderPath bit


GO
/****** Object:  StoredProcedure [dbo].[SP_CompanyGeneralSettings_Update]    Script Date: 11/13/2023 3:26:16 PM ******/
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
	@RomveFacilityNameFromOutputRootFolderPath bit =null
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
		RomveFacilityNameFromOutputRootFolderPath  = ISNULL(@RomveFacilityNameFromOutputRootFolderPath, RomveFacilityNameFromOutputRootFolderPath)
    WHERE Id = @Id
END




USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_CompanyGeneralSettings_Insert]    Script Date: 11/13/2023 3:39:31 PM ******/
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
	@RomveFacilityNameFromOutputRootFolderPath bit 
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
			RomveFacilityNameFromOutputRootFolderPath
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
			@RomveFacilityNameFromOutputRootFolderPath
		)
    
    SELECT SCOPE_IDENTITY();
END



USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrder_GetListByFilter]    Script Date: 11/13/2023 5:56:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =======================
-- Author:		Aminul
-- Create date: 22-08-2022
-- Description:	<Description,,>
-- Reference: 
-- EXEC [dbo].[SP_Order_ClientOrder_GetListByFilter] ''
-- =======================
ALTER PROCEDURE [dbo].[SP_Order_ClientOrder_GetListByFilter]
	@Where NVARCHAR(3000)='',
	@IsCalculateTotal BIT='true',
	@Skip INT = 0,
	@Top INT = 20,
	@SortColumn NVARCHAR(50) = 'o.[OrderPlaceDate]',
	@SortDirection NVARCHAR(4)='DESC'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE 
	@TotalCount INT=0,
	@TotalImageCount DECIMAL(16,0) = 0,
	@SQL NVARCHAR(MAX),
	@FinalSQL NVARCHAR(MAX),		
	@OutPut1 NVARCHAR(max)

	--get totals if page search
	IF(@IsCalculateTotal='true')
	BEGIN

			SET @SQL = N'
			SELECT 
			@TotalCount =COUNT(*),
			@TotalImageCount = ISNULL(SUM(o.[NumberOfImage]),0)
			FROM [dbo].[Order_ClientOrder] o WITH(NOLOCK) 
	left JOIN [dbo].Security_Contact assignby WITH(NOLOCK) ON assignby.Id=o.[AssignedByOpsContactId] 
	Left JOIN [dbo].Common_Company c WITH(NOLOCK) ON c.Id = o.CompanyId
	left JOIN dbo.Management_Team T WITH(NOLOCK) ON T.Id=o.AssignedTeamId '
			+@Where
		
	SET @OutPut1 = N'@TotalCount INT OUTPUT,@TotalImageCount DECIMAL(16,0) OUTPUT ';
	EXEC sp_executesql @SQL, @OutPut1, @TotalCount =@TotalCount OUTPUT,@TotalImageCount = @TotalImageCount OUTPUT;
	END	
	--select possible columns
	EXECUTE 
	('SELECT  '	
	+@TotalCount+' TotalCount, '
	+@TotalImageCount +' TotalImageCount 
	   ,o.[Id]
      ,o.[CompanyId]
	  ,c.[ObjectId] CompanyObjectId
	  ,c.[Name] CompanyName
      ,o.[FileServerId]
      ,o.[OrderNumber]
      ,o.[OrderPlaceDate]
      ,o.[ExpectedDeliveryDate]
      ,o.[ProcessingCompletedDate]
      ,o.[InternalQcRequestDate]
      ,o.[InternalQcCompleteDate]
      ,o.[ClientQcRequestDate]
      ,o.[DeliveredDate]
      ,o.[InvoiceDate]
      ,o.[ExternalOrderStatus]
      ,o.[InternalOrderStatus]
      ,o.[IsDeleted]
      ,o.[CreatedDate]
      ,o.[CreatedByContactId]
      ,o.[UpdatedDate]
      ,o.[UpdatedByContactId]
      ,o.[ObjectId]
	  ,o.[NumberOfImage]
	  ,o.[ExternalOrderStatus]
	  ,o.AllowExtraOutputFileUpload
	  ,dateadd(d, datediff(d,0, o.[OrderPlaceDate]), 0) OrderPlaceDateOnly
	  ,assignby.[FirstName] ContactName
	  ,T.[Name] TeamName
	  ,o.[AssignedDateToTeam] TeamAssignedDate
	  ,o.[ArrivalTime]
	  ,o.[DeliveryDeadlineInMinute]
	  ,c.[DeliveryDeadlineInMinute] CompanyDeliveryDeadlineInMinute
	  ,cf.Username SourceFtpUsername
	FROM [dbo].[Order_ClientOrder] o WITH(NOLOCK)
	left JOIN [dbo].Security_Contact assignby WITH(NOLOCK) ON assignby.Id=o.[AssignedByOpsContactId] 
	Left JOIN [dbo].Common_Company c WITH(NOLOCK) ON c.Id = o.CompanyId
	Left JOIN dbo.Client_ClientOrderFtp cf WITH(NOLOCK) ON cf.Id = o.SourceServerId
	left JOIN dbo.Management_Team T WITH(NOLOCK) ON T.Id=o.AssignedTeamId '
	+@Where
	
	+' ORDER BY '+@SortColumn +' '+ @SortDirection+' '
	+'OFFSET '+ @Skip+' ROWS '
	+'FETCH NEXT '+@Top+' ROWS ONLY' 
	)
END

