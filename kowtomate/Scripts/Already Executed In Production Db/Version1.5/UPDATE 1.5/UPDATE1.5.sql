ALTER TABLE CompanyGeneralSettings
ADD AllowMaxNumOfItemsPerOrder int;
ALTER TABLE CompanyGeneralSettings
ADD EnableSingleOrderPlacement bit;



CREATE PROCEDURE SP_CompanyGeneralSettings_Insert
    @AutoAssignOrderToTeam bit,
    @AutoDistributeToEditor bit,
    @AutoQcPass bit,
    @AutoOperationPass bit,
    @AllowPartialDelivery bit,
    @EnableFtpOrderPlacement bit,
    @EnableOrderDeliveryToFtp bit,
    @CompanyId int
AS
BEGIN
	DECLARE @Id int
    INSERT INTO CompanyGeneralSettings
        (AutoAssignOrderToTeam, AutoDistributeToEditor, AutoQcPass, AutoOperationPass, AllowPartialDelivery, EnableFtpOrderPlacement, EnableOrderDeliveryToFtp, CompanyId)
    VALUES
        (@AutoAssignOrderToTeam, @AutoDistributeToEditor, @AutoQcPass, @AutoOperationPass, @AllowPartialDelivery, @EnableFtpOrderPlacement, @EnableOrderDeliveryToFtp, @CompanyId)
    
SELECT SCOPE_IDENTITY();
END


CREATE PROCEDURE SP_CompanyGeneralSettings_Update
    @Id int,
    @AutoAssignOrderToTeam bit = NULL,
    @AutoDistributeToEditor bit = NULL,
    @AutoQcPass bit = NULL,
    @AutoOperationPass bit = NULL,
    @AllowPartialDelivery bit = NULL,
    @EnableFtpOrderPlacement bit = NULL,
    @EnableOrderDeliveryToFtp bit = NULL,
    @CompanyId int = NULL
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
        CompanyId = ISNULL(@CompanyId, CompanyId)
    WHERE Id = @Id
END


CREATE PROCEDURE SP_CompanyGeneralSettings_GetCompanyGeneralSettingsByCompanyId
    @CompanyId int
AS
BEGIN
    SELECT *
    FROM CompanyGeneralSettings
    WHERE CompanyId = @CompanyId
END


CREATE PROCEDURE SP_CompanyGeneralSettings_DeleteCompanyGeneralSettingsById
    @Id int
AS
BEGIN
    DELETE FROM CompanyGeneralSettings
    WHERE Id = @Id
END


CREATE PROCEDURE SP_CompanyGeneralSettings_GetCompanyGeneralSettingsById
    @Id int
AS
BEGIN
    SELECT *
    FROM CompanyGeneralSettings
    WHERE Id = @Id
END


-------------------------------------------------

CREATE PROCEDURE [dbo].[SP_Client_ClientOrderFtp_Delete_byId]
    @Id bigint
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Client_ClientOrderFtp
    WHERE Id = @Id;
END


CREATE PROCEDURE [dbo].[SP_Client_ClientOrderFtp_GetAllEnable_ClientOrderFtps]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Client_ClientOrderFtp where IsEnable=1;
END

CREATE PROCEDURE [dbo].[SP_Client_ClientOrderFtp_GetFtpInfo_byCompanyId]
    @ClientCompanyId bigint
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ClientCompanyId, Host, Port, Username, Password, IsEnable, OutputRootFolder, InputRootFolder
    FROM Client_ClientOrderFtp
    WHERE ClientCompanyId = @ClientCompanyId;
END

CREATE PROCEDURE [dbo].[SP_Client_ClientOrderFtp_Insert]
    @ClientCompanyId bigint,
    @Host varchar(255),
    @Port int,
    @Username varchar(255),
    @Password varchar(255),
    @IsEnable bit = 1,
    @OutputRootFolder varchar(max) = NULL,
    @InputRootFolder varchar(max) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Client_ClientOrderFtp (ClientCompanyId, Host, Port, Username, Password, IsEnable, OutputRootFolder, InputRootFolder)
    VALUES (@ClientCompanyId, @Host, @Port, @Username, @Password, @IsEnable, @OutputRootFolder, @InputRootFolder);
    SELECT SCOPE_IDENTITY();
END


CREATE PROCEDURE [dbo].[SP_Client_ClientOrderFtp_Update]
    @Id bigint,
    @ClientCompanyId bigint = NULL,
    @Host varchar(255) = NULL,
    @Port int = NULL,
    @Username varchar(255) = NULL,
    @Password varchar(255) = NULL,
    @IsEnable bit = NULL,
    @OutputRootFolder varchar(max) = NULL,
    @InputRootFolder varchar(max) = NULL
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
        OutputRootFolder = ISNULL(@OutputRootFolder,OutputRootFolder),
        InputRootFolder = ISNULL(@InputRootFolder,InputRootFolder)
    WHERE Id = @Id;
END


ALTER PROCEDURE [dbo].[SP_User_UpdateUserDownloadFolderPath](
       @DownloadFolderPath varchar(500),
	   @Id int,
	   @IsUserActive bit
)
AS
BEGIN  
  UPDATE [dbo].[Security_Contact]    
   SET 
	   [DownloadFolderPath] = @DownloadFolderPath,
	   IsUserActive=@IsUserActive
       WHERE Id = @Id
END