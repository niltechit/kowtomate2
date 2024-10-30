ALTER TABLE Common_Company
ADD DeliveryDeadlineInMinute int

GO

ALTER TABLE Order_ClientOrder
ADD DeliveryDeadlineInMinute int

GO

ALTER TABLE CompanyGeneralSettings
ADD FtpOrderPlacedAppNo SMALLINT

GO

ALTER TABLE Client_ClientOrderFtp
ADD IsTemporaryDeliveryUploading BIT DEFAULT 0,
	TemporaryDeliveryUploadFolder NVARCHAR(MAX);

GO
ALTER PROCEDURE [dbo].[SP_Common_Company_GetAll]
AS
BEGIN  
	SELECT 
	        Id,
			Name, 
			Code,
			CompanyType, 
			ISNULL(Telephone, '') Telephone, 
			ISNULL(Email, '') Email,
			Address1, 
			Address2, 
			City, 
			State, 
			Zipcode,
			Country,
			Status,
			CreatedDate,
			CreatedByContactId,
			UpdatedDate,
			UpdatedByContactId,
			ObjectId,
			DeliveryDeadlineInMinute
    FROM [dbo].[Common_Company] order by Id desc
END


GO

ALTER PROCEDURE [dbo].[SP_Common_Company_Insert](
            @Name  nvarchar(100),
            @Code  nvarchar(6),
            @CompanyType tinyint,
            @Telephone varchar(30),
			@Email varchar(30),
            @Address1 varchar(100),
            @Address2 varchar(100),
            @City varchar(30),
            @State varchar(30),
            @Zipcode varchar(10),
            @Country varchar(50),
            @Status int,
            @CreatedByContactId int,
            @ObjectId varchar(40),
			@FileServerId smallint,
			@DeliveryDeadlineInMinute int
)
AS
BEGIN  
    INSERT INTO [dbo].[Common_Company]
           (
			Name,
            Code,
            CompanyType,
            Telephone,
			Email,
            Address1,
            Address2,
            City,
            State,
            Zipcode,
            Country,
            Status,
			CreatedDate,
            CreatedByContactId, 
            ObjectId,
			FileServerId,
			DeliveryDeadlineInMinute
           
           )
     VALUES
          (
			@Name,
            @Code,
            @CompanyType,
            @Telephone,
			@Email,
            @Address1,
            @Address2,
            @City,
            @State,
            @Zipcode,
            @Country,
            @Status,
			SYSDATETIME(),
            @CreatedByContactId, 
            @ObjectId,
			@FileServerId,
			@DeliveryDeadlineInMinute
          
		   )

	SELECT SCOPE_IDENTITY();
END

GO


ALTER PROCEDURE [dbo].[SP_Common_Company_Update](
            @Id  int,
            @Name  nvarchar(100),
            @Code  nvarchar(6),
            @CompanyType tinyint,
            @Telephone varchar(30),
			@Email varchar(30),
            @Address1 varchar(100),
            @Address2 varchar(100),
            @City varchar(30),
            @State varchar(30),
            @Zipcode varchar(10),
            @Country varchar(50),
            @Status int,
            @UpdatedByContactId int,
			@FileServerId smallint,
			@DeliveryDeadlineInMinute INT
)
AS
BEGIN  
    UPDATE [dbo].[Common_Company]
    SET
	    Name = @Name, 
        Code = @Code,
		CompanyType= @CompanyType,
		Telephone = @Telephone,
		Email = @Email,
		Address1=@Address1,
		Address2= @Address2,
		City =@City,
		State = @State,
		Zipcode =@Zipcode ,
		Country = @Country,
		Status = @Status,
		UpdatedDate = SYSDATETIME(),
		UpdatedByContactId =@UpdatedByContactId,
		FileServerId=@FileServerId,
		DeliveryDeadlineInMinute=@DeliveryDeadlineInMinute
		
     WHERE Id = @Id
END
GO

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
	FROM [dbo].[Order_ClientOrder] o WITH(NOLOCK)
	left JOIN [dbo].Security_Contact assignby WITH(NOLOCK) ON assignby.Id=o.[AssignedByOpsContactId] 
	Left JOIN [dbo].Common_Company c WITH(NOLOCK) ON c.Id = o.CompanyId
	left JOIN dbo.Management_Team T WITH(NOLOCK) ON T.Id=o.AssignedTeamId '
	+@Where
	
	+' ORDER BY '+@SortColumn +' '+ @SortDirection+' '
	+'OFFSET '+ @Skip+' ROWS '
	+'FETCH NEXT '+@Top+' ROWS ONLY' 
	)
END

GO

ALTER PROCEDURE [dbo].[SP_Common_Company_GetAllClientCompany]
AS
BEGIN  
	SELECT 
	        Id,
			Name, 
			Code,
			CompanyType, 
			ISNULL(Telephone, '') Telephone, 
			ISNULL(Email, '') Email,
			Address1, 
			Address2, 
			City, 
			State, 
			Zipcode,
			Country,
			Status,
			CreatedDate,
			CreatedByContactId,
			UpdatedDate,
			UpdatedByContactId,
			ObjectId,
			DeliveryDeadlineInMinute
    FROM [dbo].[Common_Company] WITH(NOLOCK) WHERE Id > 2 order by Id desc
END

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
	@FtpOrderPlacedAppNo smallint
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
			FtpOrderPlacedAppNo
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
			@FtpOrderPlacedAppNo
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
	@FtpOrderPlacedAppNo smallint
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
		FtpOrderPlacedAppNo = ISNULL(@FtpOrderPlacedAppNo, FtpOrderPlacedAppNo)
    WHERE Id = @Id
END


GO
ALTER PROCEDURE [dbo].[SP_Client_ClientOrderFtp_Insert]
    @ClientCompanyId bigint,
    @Host varchar(255),
    @Port int,
    @Username varchar(255),
    @Password varchar(255),
    @IsEnable bit = 1,
    @OutputRootFolder varchar(max) = NULL,
    @InputRootFolder varchar(max) = NULL,
    @SentOutputToSeparateFTP bit = NULL,
    @OutputHost nvarchar(255)=NULL,
    @OutputUsername nvarchar(255)=NULL,
    @OutputPassword nvarchar(255)=NULL,
    @OutputPort int=NULL,
    @OutputFolderName nvarchar(max)=NULL,
	@IsTemporaryDeliveryUploading BIT=NULL,
	@TemporaryDeliveryUploadFolder NVARCHAR(MAX)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Client_ClientOrderFtp (ClientCompanyId, Host, Port, Username, Password, IsEnable, OutputRootFolder, InputRootFolder, SentOutputToSeparateFTP, OutputHost, OutputUsername, OutputPassword, OutputPort, OutputFolderName,IsTemporaryDeliveryUploading,TemporaryDeliveryUploadFolder)
    VALUES (@ClientCompanyId, @Host, @Port, @Username, @Password, @IsEnable, @OutputRootFolder, @InputRootFolder, @SentOutputToSeparateFTP, @OutputHost, @OutputUsername, @OutputPassword, @OutputPort, @OutputFolderName,@IsTemporaryDeliveryUploading,@TemporaryDeliveryUploadFolder);
    SELECT SCOPE_IDENTITY();
END

GO
ALTER PROCEDURE [dbo].[SP_Client_ClientOrderFtp_Update]
    @Id bigint,
    @ClientCompanyId bigint = NULL,
    @Host varchar(255) = NULL,
    @Port int = NULL,
    @Username varchar(255) = NULL,
    @Password varchar(255) = NULL,
    @IsEnable bit = NULL,
    @OutputRootFolder varchar(max) = NULL,
    @InputRootFolder varchar(max) = NULL,
    @SentOutputToSeparateFTP bit = NULL,
    @OutputHost nvarchar(255) = NULL,
    @OutputUsername nvarchar(255) = NULL,
    @OutputPassword nvarchar(255) = NULL,
    @OutputPort int = NULL,
    @OutputFolderName nvarchar(max) = NULL,
	@IsTemporaryDeliveryUploading BIT=NULL,
	@TemporaryDeliveryUploadFolder NVARCHAR(MAX)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Client_ClientOrderFtp
    SET 
        ClientCompanyId = ISNULL(@ClientCompanyId, ClientCompanyId),
        Host = ISNULL(@Host, Host),
        Port = ISNULL(@Port, Port),
        Username = ISNULL(@Username, Username),
        Password = ISNULL(@Password, Password),
        IsEnable = ISNULL(@IsEnable, IsEnable),
        OutputRootFolder = ISNULL(@OutputRootFolder, OutputRootFolder),
        InputRootFolder = ISNULL(@InputRootFolder, InputRootFolder),
        SentOutputToSeparateFTP = ISNULL(@SentOutputToSeparateFTP, SentOutputToSeparateFTP),
        OutputHost = ISNULL(@OutputHost, OutputHost),
        OutputUsername = ISNULL(@OutputUsername, OutputUsername),
        OutputPassword = ISNULL(@OutputPassword, OutputPassword),
        OutputPort = ISNULL(@OutputPort, OutputPort),
        OutputFolderName = ISNULL(@OutputFolderName, OutputFolderName),
		IsTemporaryDeliveryUploading = ISNULL(@IsTemporaryDeliveryUploading, IsTemporaryDeliveryUploading),
        TemporaryDeliveryUploadFolder = ISNULL(@TemporaryDeliveryUploadFolder, TemporaryDeliveryUploadFolder)
    WHERE Id = @Id;
END


GO
ALTER PROCEDURE [dbo].[SP_Client_ClientOrderFtp_GetFtpInfo_byCompanyId]
    @ClientCompanyId bigint
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ClientCompanyId, Host, Port, Username, Password, IsEnable, OutputRootFolder, InputRootFolder,SentOutputToSeparateFTP,OutputHost,OutputUsername,
	OutputPassword,OutputPort,OutputFolderName,IsTemporaryDeliveryUploading,TemporaryDeliveryUploadFolder
    FROM Client_ClientOrderFtp
    WHERE ClientCompanyId = @ClientCompanyId;
END

GO

ALTER PROCEDURE [dbo].[SP_Order_ClientOrder_Update]
(
	@Id bigint,
	@CompanyId int = NULL, 
	@FileServerId int = NULL, 
	@OrderNumber nvarchar(30) = NULL, 
	@OrderPlaceDate datetime = NULL, 
	@CreatedDate datetime = NULL, 
	@UpdatedDate datetime = NULL, 
	@UpdatedByContactId int = NULL, 
	@ObjectId nvarchar(40) = NULL, 
	@IsDeleted bit = NULL, 
	@Instructions varchar(40) = NULL,
	@DeliveryDeadlineInMinute SMALLINT = NULL
)
AS 
BEGIN 
	UPDATE Order_ClientOrder
	SET
		CompanyId = ISNULL(@CompanyId, CompanyId),
		FileServerId = ISNULL(@FileServerId, FileServerId),
		OrderNumber = ISNULL(@OrderNumber, OrderNumber),
		OrderPlaceDate = ISNULL(@OrderPlaceDate, OrderPlaceDate),
		CreatedDate = ISNULL(@CreatedDate, CreatedDate), 
		UpdatedDate = ISNULL(@UpdatedDate, UpdatedDate),
		ObjectId = ISNULL(@ObjectId, ObjectId),
		IsDeleted = ISNULL(@IsDeleted, IsDeleted),
		UpdatedByContactId = ISNULL(@UpdatedByContactId, UpdatedByContactId),
		Instructions = ISNULL(@Instructions, Instructions),
		DeliveryDeadlineInMinute = ISNULL(@DeliveryDeadlineInMinute, DeliveryDeadlineInMinute)
	WHERE Id = @Id;
END

GO

CREATE PROCEDURE [dbo].[SP_Order_ClientOrder_UpdateOrderDeadLine]
(
	@Id bigint,
	@DeliveryDeadlineInMinute SMALLINT = NULL
)
AS 
BEGIN 
	UPDATE Order_ClientOrder
	SET
		DeliveryDeadlineInMinute = ISNULL(@DeliveryDeadlineInMinute, DeliveryDeadlineInMinute)
	WHERE Id = @Id;
END

GO

USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrder_GetListByFilter]    Script Date: 7/26/2023 6:05:11 PM ******/
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
	FROM [dbo].[Order_ClientOrder] o WITH(NOLOCK)
	left JOIN [dbo].Security_Contact assignby WITH(NOLOCK) ON assignby.Id=o.[AssignedByOpsContactId] 
	Left JOIN [dbo].Common_Company c WITH(NOLOCK) ON c.Id = o.CompanyId
	left JOIN dbo.Management_Team T WITH(NOLOCK) ON T.Id=o.AssignedTeamId '
	+@Where
	
	+' ORDER BY '+@SortColumn +' '+ @SortDirection+' '
	+'OFFSET '+ @Skip+' ROWS '
	+'FETCH NEXT '+@Top+' ROWS ONLY' 
	)
END

