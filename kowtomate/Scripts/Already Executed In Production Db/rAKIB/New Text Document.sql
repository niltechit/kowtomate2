ALTER TABLE [dbo].[Client_ClientOrderFtp]
ADD DeliveryDeadlineInMinute DECIMAL(18, 2);




GO
/****** Object:  StoredProcedure [dbo].[SP_Client_ClientOrderFtp_Update]    Script Date: 12/22/2023 1:58:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
    @OutputProtocolTypePuttyKeyPath varchar(255) = NULL,
	@DeliveryDeadlineInMinute decimal
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
            OutputProtocolTypePuttyKeyPath = ISNULL(@OutputProtocolTypePuttyKeyPath, OutputProtocolTypePuttyKeyPath),
			DeliveryDeadlineInMinute = ISNULL(@DeliveryDeadlineInMinute, DeliveryDeadlineInMinute)
        WHERE Id = @Id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Client_ClientOrderFtp_Insert]    Script Date: 12/22/2023 2:08:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
    @OutputProtocolTypePuttyKeyPath varchar(255) = NULL,
	@DeliveryDeadlineInMinute decimal
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
            OutputProtocolTypePuttyKeyPath,
			DeliveryDeadlineInMinute
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
            @OutputProtocolTypePuttyKeyPath,
			@DeliveryDeadlineInMinute
        );
        SELECT SCOPE_IDENTITY();
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END


/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrder_UpdateOrderDeadLine]    Script Date: 12/26/2023 4:38:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_Order_ClientOrder_UpdateOrderDeadLine]
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
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrder_UpdateOrderDeadLine]    Script Date: 12/26/2023 1:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE [dbo].[SP_Order_ClientOrder_UpdateOrderDeadLineDate]
(
	@Id bigint,
	@ExpectedDeliveryDate datetime = null
)
AS 
BEGIN 
	UPDATE Order_ClientOrder
	SET
		ExpectedDeliveryDate = ISNULL(@ExpectedDeliveryDate, ExpectedDeliveryDate)
	WHERE Id = @Id;
END


Alter table Order_ClientOrderItem add ExpectedDeliveryDate datetime

GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrder_UpdateOrderDeadLine]    Script Date: 12/26/2023 1:03:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE [dbo].[SP_Order_ClientOrderItem_UpdateOrderItemDeadLineDate]
(
	@ClientOrderId bigint,
	@ExpectedDeliveryDate datetime = null
)
AS 
BEGIN 
	UPDATE Order_ClientOrderItem
	SET
		ExpectedDeliveryDate = ISNULL(@ExpectedDeliveryDate, ExpectedDeliveryDate)
	WHERE ClientOrderId = @ClientOrderId;
END


GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItemsMinDeliveryDateByOrderId]    Script Date: 12/26/2023 5:27:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItemsMinDeliveryDateByOrderId] 
	@ClientOrderId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT  Min(ExpectedDeliveryDate) MinDeliveryDate,Min(ExpectedDeliveryDate) MaxDeliveryDate from [dbo].[Order_ClientOrderItem] where ClientOrderId=@ClientOrderId
END


