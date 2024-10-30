ALTER TABLE Client_ClientOrderFtp
ADD SentOutputToSeparateFTP BIT DEFAULT 0,
    OutputHost NVARCHAR(255),
    OutputUsername NVARCHAR(255),
    OutputPassword NVARCHAR(1255),
    OutputPort INT,
    OutputFolderName NVARCHAR(MAX);

GO
ALTER PROCEDURE [dbo].[SP_Client_ClientOrderFtp_GetFtpInfo_byCompanyId]
    @ClientCompanyId bigint
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ClientCompanyId, Host, Port, Username, Password, IsEnable, OutputRootFolder, InputRootFolder, SentOutputToSeparateFTP,OutputHost,OutputUsername,
	OutputPassword,OutputPort,OutputFolderName
    FROM Client_ClientOrderFtp
    WHERE ClientCompanyId = @ClientCompanyId;
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
    @OutputFolderName nvarchar(max)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Client_ClientOrderFtp (ClientCompanyId, Host, Port, Username, Password, IsEnable, OutputRootFolder, InputRootFolder, SentOutputToSeparateFTP, OutputHost, OutputUsername, OutputPassword, OutputPort, OutputFolderName)
    VALUES (@ClientCompanyId, @Host, @Port, @Username, @Password, @IsEnable, @OutputRootFolder, @InputRootFolder, @SentOutputToSeparateFTP, @OutputHost, @OutputUsername, @OutputPassword, @OutputPort, @OutputFolderName);
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
    @OutputFolderName nvarchar(max) = NULL
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
        OutputFolderName = ISNULL(@OutputFolderName, OutputFolderName)
    WHERE Id = @Id;
END
