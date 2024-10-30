--1 Add Field Type

ALTER TABLE Order_ClientOrderItem
	add FileGroup tinyint null

	Go
	update Order_ClientOrderItem set FileGroup = 1
	Go

	ALTER TABLE Order_ClientOrderItem
	alter column FileGroup tinyint not null ------- add by rakib 26/12/2022 12.32 PM .




GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_StatusUpdate]    Script Date: 12/26/2022 1:48:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItem_StatusUpdate]
(
	@Id bigint,
	@Status tinyint,
	@ExternalStatus tinyint,
	@FileGroup tinyint
	
)
as 
begin 
	update Order_ClientOrderItem
	set
	[Status] = @Status,
	ExternalStatus = @ExternalStatus,
	[FileGroup] = @FileGroup

	where Id=@Id
end




GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetAllByOrderId]    Script Date: 12/26/2022 6:33:54 PM ******/
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
      ,[FileSize]
	  ,orderitem.FileByteString
	  ,orderitem.Status as Status
	  ,orderitem.ExternalStatus
	  ,orderitem.FileGroup
	  ,assignorder.AssignDate as OrderAssignDate
	  ,contact.FirstName as EditorFirstName
	  ,contact.LastName as EditorLastName
	  ,mt.Name as TeamName
  FROM [dbo].[Order_ClientOrderItem] as orderitem 
  left join dbo.Order_AssignedImageEditor as assignorder With(Nolock) on orderitem.Id = assignorder.Order_ImageId 
  left join dbo.Security_Contact as contact With(Nolock) on contact.Id=assignorder.AssignContactId
  left join dbo.Management_Team as mt With(Nolock) on mt.Id = orderitem.TeamId
  
   where ClientOrderId=@OrderId and (IsDeleted=0 or IsDeleted is null)


END




GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderAssignedItem_GetByOrderIdContactIdTeamId]    Script Date: 12/26/2022 6:36:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
	  ,orderitem.FileGroup
	  ,assignorder.AssignDate as OrderAssignDate
	  ,contact.FirstName as EditorFirstName
	  ,contact.LastName as EditorLastName
	  ,assignorder.AssignContactId
	  ,mt.Name as TeamName
	from Order_ClientOrderItem as orderitem 
	left join Order_AssignedImageEditor as assignorder With(NoLock) on orderitem.Id = assignorder.Order_ImageId
	left join dbo.Security_Contact as contact With(NoLock) on contact.Id=assignorder.AssignContactId
	left join dbo.Management_Team as mt With(NoLock) on mt.Id = orderitem.TeamId
	where orderitem.ClientOrderId=@OrderId and  orderitem.TeamId = @TeamId
end



GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderAssignedItem_GetByOrderId]    Script Date: 12/26/2022 6:38:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
	  ,assignorder.AssignDate as OrderAssignDate
	  ,contact.FirstName as EditorFirstName
	  ,contact.LastName as EditorLastName
	  ,orderitem.ProductionDoneFilePath
	  ,assignorder.AssignContactId
	  ,mt.Name as TeamName
	from Order_ClientOrderItem as orderitem 
	inner join Order_AssignedImageEditor as assignorder WITH(NOLOCK) on orderitem.Id = assignorder.Order_ImageId
	inner join dbo.Security_Contact as contact  WITH(NOLOCK) on contact.Id=assignorder.AssignContactId
	left join dbo.Management_Team as mt  WITH(NOLOCK) on mt.Id = orderitem.TeamId
	where orderitem.ClientOrderId=@OrderId and assignorder.AssignContactId = @ContactId 
end
	



GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetEqualAndGreaterItemByStatus]    Script Date: 12/26/2022 6:39:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
	  ,assignorder.AssignDate as OrderAssignDate
	  ,contact.FirstName as EditorFirstName
	  ,contact.LastName as EditorLastName
	  ,assignorder.AssignContactId
	   ,mt.Name as TeamName
	from dbo.Order_ClientOrderItem as orderitem 
	left join Order_AssignedImageEditor as assignorder WITH(NOLOCK) on orderitem.Id = assignorder.Order_ImageId
	left join dbo.Security_Contact as contact WITH(NOLOCK) on contact.Id=assignorder.AssignContactId
	left join dbo.Management_Team as mt WITH(NOLOCK) on mt.Id = orderitem.TeamId
	
	 where ClientOrderId = @OrderId and orderitem.Status >= @Status
end






GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_insert]    Script Date: 12/26/2022 7:02:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItem_insert]
(
	
	@CompanyId int,
	@FileName nvarchar(max),
	@ClientOrderId int,

	@PartialPath nvarchar(250),
	@Status int,
	@IsDeleted bit,
	@CreatedDate datetime,
	@UpdatedDate datetime,
	@ObjectId nvarchar(max),
	@FileSize bigint,
	@TeamId int,
	@ExternalStatus int,
	@FileByteString nvarchar(max),
	@InternalFileOutputPath nvarchar(max),
	@InternalFileInputPath nvarchar(max),
	@ExternalFileOutputPath nvarchar(max),
	@FileNameWithoutExtension nvarchar(200),
	@FileGroup int
)
as
begin
	insert into 
	Order_ClientOrderItem([FileName],ClientOrderId,[Status],IsDeleted, CreatedDate,UpdatedDate,ObjectId,FileSize,ExternalStatus,FileByteString,InternalFileOutputPath,InternalFileInputPath,ExternalFileOutputPath,CompanyId,PartialPath,FileNameWithoutExtension,[FileGroup])
					  
	values(@FileName,@ClientOrderId,@Status,@IsDeleted, @CreatedDate,@UpdatedDate,@ObjectId,@FileSize,@ExternalStatus,@FileByteString,@InternalFileOutputPath,@InternalFileInputPath,@ExternalFileOutputPath,@CompanyId,@PartialPath,@FileNameWithoutExtension,@FileGroup)

    SELECT SCOPE_IDENTITY();
end




GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItemsMinStatusByOrderId]    Script Date: 12/27/2022 1:01:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- Author:		Md Rakib Hossain
-- Create date: 10-11-2022
-- Description:	Client Order Items MinStatusByOrderId
-- =============================================
ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItemsMinStatusByOrderId] 
	@OrderId int,
	@FileGroup int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT  Min(Status) Status,Min(ExternalStatus) ExternalStatus from [dbo].[Order_ClientOrderItem] where ClientOrderId=@OrderId and FileGroup = @FileGroup
END

























