
GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetAllByOrderId]    Script Date: 12/27/2023 3:00:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:	Md Zakir Hossain	
-- Create date: 09 Sept 2022
-- Description:	Get All Order Item by Order Id
-- =============================================

ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetAllByOrderId]
	@OrderId int
AS
BEGIN  
	
	
	SELECT orderitem.[Id]
      ,orderitem.[CompanyId]
      ,[ClientOrderId]
      ,[FileName]
      ,[ExteranlFileInputPath]
      ,[ExternalFileOutputPath]
	  ,[InternalFileInputPath]
      ,[InternalFileOutputPath]
	  ,[PartialPath]
      ,[UnitPrice]
      ,orderitem.[IsDeleted]
      ,orderitem.[CreatedDate]
      ,orderitem.[CreatedByContactId]
      ,orderitem.[UpdatedDate]
      ,orderitem.[UpdatedByContactId]
      ,orderitem.[ObjectId]
	  ,orderitem.TeamId
	  ,orderitem.ProductionDoneFilePath
	  ,orderitem.ProductionDoneFilePath
	  ,orderitem.FileNameWithoutExtension
	  ,orderitem.ExpectedDeliveryDate
      ,[FileSize]
	  ,orderitem.FileByteString
	  ,orderitem.Status as Status
	  ,orderitem.ExternalStatus
	  ,orderitem.FileGroup
	  ,assignorder.AssignDate as OrderAssignDate
	  ,contact.FirstName as EditorFirstName
	  ,contact.LastName as EditorLastName
	  ,mt.Name as TeamName,
	  orderitem.ArrivalTime
  FROM [dbo].[Order_ClientOrderItem] as orderitem 
  left join dbo.Order_AssignedImageEditor as assignorder With(Nolock) on orderitem.Id = assignorder.Order_ImageId and assignorder.IsActive = 1
  left join dbo.Security_Contact as contact With(Nolock) on contact.Id=assignorder.AssignContactId
  left join dbo.Management_Team as mt With(Nolock) on mt.Id = orderitem.TeamId
  
   where ClientOrderId=@OrderId and (orderitem.IsDeleted=0 or orderItem.IsDeleted is null)  and orderItem.FileGroup <> 4 --4 means ColorRef


END




go 


ALTER PROCEDURE [dbo].[SP_Order_ClientOrderAssignedItem_GetByOrderIdContactIdTeamId]
(
	@OrderId int,
	@ContactId int,
	@TeamId int
)
as
begin
	SELECT orderitem.[Id]
      ,orderitem.[CompanyId]
      ,[ClientOrderId]
      ,[FileName]
      ,[ExteranlFileInputPath]
      ,[ExternalFileOutputPath]
	  ,[InternalFileInputPath]
      ,[InternalFileOutputPath]
	  ,[PartialPath]
      ,[UnitPrice]
      ,orderitem.[IsDeleted]
      ,orderitem.[CreatedDate]
      ,orderitem.[CreatedByContactId]
      ,orderitem.[UpdatedDate]
      ,orderitem.[UpdatedByContactId]
      ,orderitem.[ObjectId]
      ,orderitem.[FileSize] as FileSize
	  ,orderitem.TeamId
	  ,orderitem.Status as Status
	  ,orderitem.ExternalStatus
	  ,orderitem.FileByteString
	  ,orderitem.ProductionFileByteString
	  ,orderitem.ProductionDoneFilePath
	  ,orderitem.ExpectedDeliveryDate
	  ,orderitem.FileGroup
	  ,assignorder.AssignDate as OrderAssignDate
	  ,contact.FirstName as EditorFirstName
	  ,contact.LastName as EditorLastName
	  ,assignorder.AssignContactId
	  ,mt.Name as TeamName
	from Order_ClientOrderItem as orderitem 
	left join Order_AssignedImageEditor as assignorder With(NoLock) on orderitem.Id = assignorder.Order_ImageId and assignorder.IsActive = 1
	left join dbo.Security_Contact as contact With(NoLock) on contact.Id=assignorder.AssignContactId
	left join dbo.Management_Team as mt With(NoLock) on mt.Id = orderitem.TeamId
	where orderitem.ClientOrderId=@OrderId  and orderitem.TeamId = @TeamId and orderItem.FileGroup <> 4 --4 means ColorRef
end





GO
ALTER PROCEDURE [dbo].[SP_Order_ClientOrderAssignedItem_GetByOrderId]
(
	@OrderId int,
	@ContactId int
	
)
as
begin
	SELECT orderitem.[Id]
      ,orderitem.[CompanyId]
      ,[ClientOrderId]
      ,[FileName]
      ,[ExteranlFileInputPath]
	  ,[InternalFileInputPath]
      ,[ExternalFileOutputPath]
      ,[InternalFileOutputPath]
	  ,[PartialPath]
      ,[UnitPrice]
      ,orderitem.[IsDeleted]
      ,orderitem.[CreatedDate]
      ,orderitem.[CreatedByContactId]
      ,orderitem.[UpdatedDate]
      ,orderitem.[UpdatedByContactId]
      ,orderitem.[ObjectId]
      ,orderitem.[FileSize] as FileSize
	  ,orderitem.TeamId
	  ,orderitem.Status as Status
	  ,orderitem.ExternalStatus
	  ,orderitem.ProductionFileByteString
	  ,orderitem.FileByteString
	  ,orderitem.FileGroup
	   ,orderitem.ExpectedDeliveryDate
	  ,assignorder.AssignDate as OrderAssignDate
	  ,contact.FirstName as EditorFirstName
	  ,contact.LastName as EditorLastName
	  ,orderitem.ProductionDoneFilePath
	  ,assignorder.AssignContactId
	  ,mt.Name as TeamName
	  ,orderitem.ArrivalTime
	from Order_ClientOrderItem as orderitem 
	inner join Order_AssignedImageEditor as assignorder WITH(NOLOCK) on orderitem.Id = assignorder.Order_ImageId and assignorder.IsActive = 1 
	inner join dbo.Security_Contact as contact  WITH(NOLOCK) on contact.Id=assignorder.AssignContactId
	left join dbo.Management_Team as mt  WITH(NOLOCK) on mt.Id = orderitem.TeamId
	where orderitem.ClientOrderId=@OrderId and assignorder.AssignContactId = @ContactId and orderitem.FileGroup <> 4 --4 means ColorRef
end


GO
ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetEqualAndGreaterItemByStatus]
(
	@OrderId int,
	@Status tinyint
)
as
begin
	select orderitem.[Id]
      ,orderitem.[CompanyId]
      ,[ClientOrderId]
      ,[FileName]
      ,[ExteranlFileInputPath]
      ,[ExternalFileOutputPath]
	  ,[InternalFileInputPath]
      ,[InternalFileOutputPath]
	  ,[PartialPath]
      ,[UnitPrice]
      ,orderitem.[IsDeleted]
      ,orderitem.[CreatedDate]
      ,orderitem.[CreatedByContactId]
      ,orderitem.[UpdatedDate]
      ,orderitem.[UpdatedByContactId]
      ,orderitem.[ObjectId]
      ,orderitem.[FileSize] as FileSize
	  ,orderitem.TeamId
	  ,orderitem.Status as Status
	  ,orderitem.ExternalStatus
	  ,orderitem.FileByteString
	  ,orderitem.ProductionFileByteString
	  ,orderitem.ProductionDoneFilePath
	  ,orderitem.FileGroup
	  ,orderitem.ExpectedDeliveryDate
	  ,assignorder.AssignDate as OrderAssignDate
	  ,contact.FirstName as EditorFirstName
	  ,contact.LastName as EditorLastName
	  ,assignorder.AssignContactId
	   ,mt.Name as TeamName
	   ,orderitem.ArrivalTime
	from dbo.Order_ClientOrderItem as orderitem 
	left join Order_AssignedImageEditor as assignorder WITH(NOLOCK) on orderitem.Id = assignorder.Order_ImageId and assignorder.IsActive = 1
	left join dbo.Security_Contact as contact WITH(NOLOCK) on contact.Id=assignorder.AssignContactId
	left join dbo.Management_Team as mt WITH(NOLOCK) on mt.Id = orderitem.TeamId
	
	 where ClientOrderId = @OrderId and orderitem.Status >= @Status and orderItem.FileGroup <> 4 --4 means ColorRef
end

go


ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetAllByOrderId]
	@OrderId int
AS
BEGIN  
	
	
	SELECT orderitem.[Id]
      ,orderitem.[CompanyId]
      ,[ClientOrderId]
      ,[FileName]
      ,[ExteranlFileInputPath]
      ,[ExternalFileOutputPath]
	  ,[InternalFileInputPath]
      ,[InternalFileOutputPath]
	  ,[PartialPath]
      ,[UnitPrice]
      ,orderitem.[IsDeleted]
      ,orderitem.[CreatedDate]
      ,orderitem.[CreatedByContactId]
      ,orderitem.[UpdatedDate]
      ,orderitem.[UpdatedByContactId]
      ,orderitem.[ObjectId]
	  ,orderitem.TeamId
	  ,orderitem.ProductionDoneFilePath
	  ,orderitem.ProductionDoneFilePath
	  ,orderitem.FileNameWithoutExtension
	  ,orderitem.ExpectedDeliveryDate
      ,[FileSize]
	  ,orderitem.FileByteString
	  ,orderitem.Status as Status
	  ,orderitem.ExternalStatus
	  ,orderitem.FileGroup
	  ,orderitem.ExpectedDeliveryDate
	  ,assignorder.AssignDate as OrderAssignDate
	  ,contact.FirstName as EditorFirstName
	  ,contact.LastName as EditorLastName
	  ,mt.Name as TeamName,
	  orderitem.ArrivalTime
  FROM [dbo].[Order_ClientOrderItem] as orderitem 
  left join dbo.Order_AssignedImageEditor as assignorder With(Nolock) on orderitem.Id = assignorder.Order_ImageId and assignorder.IsActive = 1
  left join dbo.Security_Contact as contact With(Nolock) on contact.Id=assignorder.AssignContactId
  left join dbo.Management_Team as mt With(Nolock) on mt.Id = orderitem.TeamId
  
   where ClientOrderId=@OrderId and (orderitem.IsDeleted=0 or orderItem.IsDeleted is null)  and orderItem.FileGroup <> 4 --4 means ColorRef


END






GO
/****** Object:  StoredProcedure [dbo].[SP_Client_ClientOrderFtp_Update]    Script Date: 1/8/2024 2:41:20 PM ******/
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
			DeliveryDeadlineInMinute = @DeliveryDeadlineInMinute
        WHERE Id = @Id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END





















