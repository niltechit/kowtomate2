
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
	@AllowNotifyOpsOnImageArrivalFTP bit
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
			AllowNotifyOpsOnImageArrivalFTP
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
			@AllowNotifyOpsOnImageArrivalFTP
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
	@AllowSingleOrderFromFTP bit,
	@AllowMaxNumOfItemsPerOrder int,
	@EnableSingleOrderPlacement bit,
	@IsIbrProcessedEnabled bit,
	@AllowAutoUploadFromEditorPc bit,
	@AllowAutoUploadFromQcPc bit,
	@AllowAutoDownloadToEditorPc bit,
	@AllowClientWiseImageProcessing bit,
	@AllowNotifyOpsOnImageArrivalFTP bit
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
		AllowSingleOrderFromFTP=ISNULL(@AllowSingleOrderFromFTP,AllowSingleOrderFromFTP),
		EnableSingleOrderPlacement=ISNULL(@EnableSingleOrderPlacement,EnableSingleOrderPlacement),
		AllowMaxNumOfItemsPerOrder=ISNULL(@AllowMaxNumOfItemsPerOrder,AllowMaxNumOfItemsPerOrder),
		IsIbrProcessedEnabled=ISNULL(@IsIbrProcessedEnabled,IsIbrProcessedEnabled),
		AllowAutoUploadFromEditorPc=ISNULL(@AllowAutoUploadFromEditorPc,AllowAutoUploadFromEditorPc),
		AllowAutoUploadFromQcPc=ISNULL(@AllowAutoUploadFromQcPc,AllowAutoUploadFromQcPc),
		AllowAutoDownloadToEditorPc=ISNULL(@AllowAutoDownloadToEditorPc,AllowAutoDownloadToEditorPc),
		AllowClientWiseImageProcessing=ISNULL(@AllowClientWiseImageProcessing,AllowClientWiseImageProcessing),
		AllowNotifyOpsOnImageArrivalFTP=ISNULL(@AllowNotifyOpsOnImageArrivalFTP,AllowNotifyOpsOnImageArrivalFTP)

    WHERE Id = @Id
END

GO

GO
ALTER PROCEDURE [dbo].[SP_Security_GetSecurityLoginHistories]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	 SELECT Id, UserId, Username, ActionTime, 

	CASE WHEN  ActionType=1 THEN 'Login' ELSE 'Logout' end as ActionType,
	 
	 IPAddress, DeviceInfo, Status
    FROM Security_LoginHistory order by ActionTime desc
END

GO

ALTER PROCEDURE [dbo].[SP_Security_InsertSecurityLoginHistory] 
	-- Add the parameters for the stored procedure here
	@UserId INT,
	@Username VARCHAR(50),
    @ActionTime DATETIME,
    @ActionType BIT,
    @IPAddress VARCHAR(50),
    @DeviceInfo VARCHAR(50),
    @Status BIT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO Security_LoginHistory 
	(
		UserId, 
		Username, 
		ActionTime, 
		ActionType, 
		IPAddress, 
		DeviceInfo, 
		Status
	)
    VALUES 
	(
		@UserId, 
		@Username, 
		@ActionTime, 
		@ActionType, 
		@IPAddress, 
		@DeviceInfo, 
		@Status
	)
	SELECT SCOPE_IDENTITY()
END

GO
ALTER PROCEDURE [dbo].[SP_Security_UpdateSecurityLoginHistoryById]
	@Id INT,
	@UserId INT=NULL,
	@Username VARCHAR(50)=NULL,
	@ActionTime DATETIME=NULL,
	@ActionType BIT=NULL,
	@IPAddress VARCHAR(50)=NULL,
	@DeviceInfo VARCHAR(50)=NULL,
	@Status BIT=NULL
AS
BEGIN
	UPDATE [dbo].[Security_LoginHistory]
	SET
		[UserId] = ISNULL(@UserId, [UserId]),
		[Username] = ISNULL(@Username, [Username]),
		[ActionTime] = ISNULL(@ActionTime, [ActionTime]),
		[ActionType] = ISNULL(@ActionType, [ActionType]),
		[IPAddress] = ISNULL(@IPAddress, [IPAddress]),
		[DeviceInfo] = ISNULL(@DeviceInfo, [DeviceInfo]),
		[Status] = ISNULL(@Status, [Status])
	WHERE
		[Id] = @Id;
END
