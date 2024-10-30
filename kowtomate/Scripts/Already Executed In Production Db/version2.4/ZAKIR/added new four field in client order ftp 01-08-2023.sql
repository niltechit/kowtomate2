ALTER TABLE Client_ClientOrderFtp
ADD InputProtocolType SMALLINT,
    OutputProtocolType SMALLINT,
	InputPassPhrase NVARCHAR(500),
	OutputPassPhrase NVARCHAR(500),
	InputProtocolTypePuttyKeyPath NVARCHAR(MAX),
	OutputProtocolTypePuttyKeyPath NVARCHAR(MAX);

GO
 
 ALTER TABLE Client_ClientOrderFtp
 ALTER COLUMN  [Password] VARCHAR(255) null

go

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
    @OutputHost nvarchar(255) = NULL,
    @OutputUsername nvarchar(255) = NULL,
    @OutputPassword nvarchar(255) = NULL,
    @OutputPort int = NULL,
    @OutputFolderName nvarchar(max) = NULL,
    @IsTemporaryDeliveryUploading BIT = NULL,
    @TemporaryDeliveryUploadFolder NVARCHAR(MAX) = NULL,
    @IsDefault BIT,
    @InputProtocolType smallint = NULL,
    @OutputProtocolType smallint = NULL,
    @InputPassPhrase varchar(255) = NULL,
    @OutputPassPhrase varchar(255) = NULL,
    @InputProtocolTypePuttyKeyPath varchar(255) = NULL,
    @OutputProtocolTypePuttyKeyPath varchar(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF (@IsDefault = 1)
        BEGIN
            UPDATE Client_ClientOrderFtp
            SET IsDefault = 0;
        END

        INSERT INTO Client_ClientOrderFtp (
            ClientCompanyId,
            Host,
            Port,
            Username,
            Password,
            IsEnable,
            OutputRootFolder,
            InputRootFolder,
            SentOutputToSeparateFTP,
            OutputHost,
            OutputUsername,
            OutputPassword,
            OutputPort,
            OutputFolderName,
            IsTemporaryDeliveryUploading,
            TemporaryDeliveryUploadFolder,
            IsDefault,
            InputProtocolType,
            OutputProtocolType,
            InputPassPhrase,
            OutputPassPhrase,
            InputProtocolTypePuttyKeyPath,
            OutputProtocolTypePuttyKeyPath
        )
        VALUES (
            @ClientCompanyId,
            @Host,
            @Port,
            @Username,
            @Password,
            @IsEnable,
            @OutputRootFolder,
            @InputRootFolder,
            @SentOutputToSeparateFTP,
            @OutputHost,
            @OutputUsername,
            @OutputPassword,
            @OutputPort,
            @OutputFolderName,
            @IsTemporaryDeliveryUploading,
            @TemporaryDeliveryUploadFolder,
            @IsDefault,
            @InputProtocolType,
            @OutputProtocolType,
            @InputPassPhrase,
            @OutputPassPhrase,
            @InputProtocolTypePuttyKeyPath,
            @OutputProtocolTypePuttyKeyPath
        );
        SELECT SCOPE_IDENTITY();
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
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
    @IsTemporaryDeliveryUploading BIT = NULL,
    @TemporaryDeliveryUploadFolder NVARCHAR(MAX) = NULL,
    @IsDefault BIT = NULL,
    @InputProtocolType smallint = NULL,
    @OutputProtocolType smallint = NULL,
    @InputPassPhrase varchar(255) = NULL,
    @OutputPassPhrase varchar(255) = NULL,
    @InputProtocolTypePuttyKeyPath varchar(255) = NULL,
    @OutputProtocolTypePuttyKeyPath varchar(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF (@IsDefault = 1)
        BEGIN
            UPDATE Client_ClientOrderFtp
            SET IsDefault = 0;
        END

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
            TemporaryDeliveryUploadFolder = ISNULL(@TemporaryDeliveryUploadFolder, TemporaryDeliveryUploadFolder),
            IsDefault = ISNULL(@IsDefault, IsDefault),
            InputProtocolType = ISNULL(@InputProtocolType, InputProtocolType),
            OutputProtocolType = ISNULL(@OutputProtocolType, OutputProtocolType),
            InputPassPhrase = ISNULL(@InputPassPhrase, InputPassPhrase),
            OutputPassPhrase = ISNULL(@OutputPassPhrase, OutputPassPhrase),
            InputProtocolTypePuttyKeyPath = ISNULL(@InputProtocolTypePuttyKeyPath, InputProtocolTypePuttyKeyPath),
            OutputProtocolTypePuttyKeyPath = ISNULL(@OutputProtocolTypePuttyKeyPath, OutputProtocolTypePuttyKeyPath)
        WHERE Id = @Id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
