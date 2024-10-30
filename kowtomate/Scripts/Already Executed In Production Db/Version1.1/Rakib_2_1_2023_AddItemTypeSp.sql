USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetByClientOrderIdAndFileGroup]    Script Date: 1/2/2023 12:43:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rakib
-- Create date: 2 Jan 2023
-- Description:	Get OrderItem Using OrderId and FileGroup  [SP_Order_ClientOrder_AllowExtraOutputFileUploadFieldUpdate]
-- =============================================
Create PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetByClientOrderIdAndFileGroup]
(
	@FileGroup int,
	@ClientOrderId int
)
as
begin
	select * from Order_ClientOrderItem where FileGroup = @FileGroup and ClientOrderId = @ClientOrderId
end




GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetAllByOrderId]    Script Date: 1/2/2023 1:37:40 PM ******/
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
  
   where ClientOrderId=@OrderId and (IsDeleted=0 or IsDeleted is null) and orderItem.FileGroup <> 4 --4 means ColorRef


END



GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderAssignedItem_GetByOrderIdContactIdTeamId]    Script Date: 1/2/2023 2:06:07 PM ******/
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
	where orderitem.ClientOrderId=@OrderId and  orderitem.TeamId = @TeamId and orderItem.FileGroup <> 4 --4 means ColorRef
end




GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderAssignedItem_GetByOrderId]    Script Date: 1/2/2023 2:12:35 PM ******/
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
	where orderitem.ClientOrderId=@OrderId and assignorder.AssignContactId = @ContactId and orderitem.FileGroup <> 4 --4 means ColorRef
end





GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetEqualAndGreaterItemByStatus]    Script Date: 1/2/2023 2:18:42 PM ******/
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
	
	 where ClientOrderId = @OrderId and orderitem.Status >= @Status and orderItem.FileGroup <> 4 --4 means ColorRef
end


