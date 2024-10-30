alter  table dbo.Order_AssignedImageEditor add IsActive bit not null default (1)


GO
/****** Object:  StoredProcedure [dbo].[SP_Order_Order_AssignedImageEditor_DeleteByAssignContactIdAndOrderImageId]    Script Date: 5/25/2023 4:28:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rakib
-- Create date: 13 November 2022
-- Description:	Delete AssignedImageEditor info 
-- =============================================
ALTER PROCEDURE [dbo].[SP_Order_Order_AssignedImageEditor_DeleteByAssignContactIdAndOrderImageId]
(
	@OrderItemId int
)
AS
BEGIN  
    update [dbo].Order_AssignedImageEditor set IsActive = 0 WHERE Order_ImageId = @OrderItemId
END


GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetAssignOrderItemByContactIdAndTeamId]    Script Date: 5/25/2023 4:30:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Author:		Rakib
-- Create date:4/14/2023
-- Description:	Get An Editor all assigned image which exist in inproduction,distribute, reworkinproduction, reworkdistributation

-- =============================================
ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetAssignOrderItemByContactIdAndTeamId]
	@teamId int,
	@ContactId int
AS
BEGIN

	    SELECT a.OrderId into #oIds FROM  [dbo].[Order_Assigned_Team] a WITH(NOLOCK)
                    INNER JOIN Management_TeamMember m WITH(NOLOCK) ON m.TeamId = a.TeamId
                    WHERE a.TeamId = @teamId
		SELECT ci.*
		FROM [dbo].[Order_ClientOrderItem] as ci inner join dbo.Order_AssignedImageEditor as aie on ci.Id = aie.Order_ImageId and aie.IsActive = 1 where AssignContactId = @ContactId and Status in (7,8,9,11,12) and  ci.ClientOrderId in (select OrderId from #oIds) 
		

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderAssignedItem_GetByOrderId]    Script Date: 5/25/2023 4:37:13 PM ******/
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
	  ,orderitem.ArrivalTime
	from Order_ClientOrderItem as orderitem 
	inner join Order_AssignedImageEditor as assignorder WITH(NOLOCK) on orderitem.Id = assignorder.Order_ImageId and assignorder.IsActive = 1 
	inner join dbo.Security_Contact as contact  WITH(NOLOCK) on contact.Id=assignorder.AssignContactId
	left join dbo.Management_Team as mt  WITH(NOLOCK) on mt.Id = orderitem.TeamId
	where orderitem.ClientOrderId=@OrderId and assignorder.AssignContactId = @ContactId and orderitem.FileGroup <> 4 --4 means ColorRef
end




GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetAllByOrderId]    Script Date: 5/25/2023 4:39:13 PM ******/
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
	  ,mt.Name as TeamName,
	  orderitem.ArrivalTime
  FROM [dbo].[Order_ClientOrderItem] as orderitem 
  left join dbo.Order_AssignedImageEditor as assignorder With(Nolock) on orderitem.Id = assignorder.Order_ImageId and assignorder.IsActive = 1
  left join dbo.Security_Contact as contact With(Nolock) on contact.Id=assignorder.AssignContactId
  left join dbo.Management_Team as mt With(Nolock) on mt.Id = orderitem.TeamId
  
   where ClientOrderId=@OrderId and (IsDeleted=0 or IsDeleted is null)  and orderItem.FileGroup <> 4 --4 means ColorRef


END




GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderAssignedItem_GetByOrderIdContactIdTeamId]    Script Date: 5/25/2023 4:40:36 PM ******/
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
	left join Order_AssignedImageEditor as assignorder With(NoLock) on orderitem.Id = assignorder.Order_ImageId and assignorder.IsActive = 1
	left join dbo.Security_Contact as contact With(NoLock) on contact.Id=assignorder.AssignContactId
	left join dbo.Management_Team as mt With(NoLock) on mt.Id = orderitem.TeamId
	where orderitem.ClientOrderId=@OrderId  and orderitem.TeamId = @TeamId and orderItem.FileGroup <> 4 --4 means ColorRef
end





GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetAllByOrderIdAndStatus]    Script Date: 5/25/2023 4:43:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:	Rakib	
-- Create date: 03 Jan 2023
-- Description:	Get All Order Item by Order Id and Status
-- =============================================

ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetAllByOrderIdAndStatus]
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
	  ,orderitem.ArrivalTime
  FROM [dbo].[Order_ClientOrderItem] as orderitem 
  left join dbo.Order_AssignedImageEditor as assignorder With(Nolock) on orderitem.Id = assignorder.Order_ImageId  and assignorder.IsActive = 1
  left join dbo.Security_Contact as contact With(Nolock) on contact.Id=assignorder.AssignContactId
  left join dbo.Management_Team as mt With(Nolock) on mt.Id = orderitem.TeamId
  
   where ClientOrderId=@OrderId and (IsDeleted=0 or IsDeleted is null) and orderItem.FileGroup <> 4 and orderItem.ExternalStatus in (24,26)--4 means ColorRef ,,, -- 24 = Ready To Download and 26 = Completed


END

GO


ALTER TABLE Security_Contact ADD MinPerImage DECIMAL(5, 3) default 3




GO
/****** Object:  StoredProcedure [dbo].[SP_OrderItem_GetListByFilter]    Script Date: 5/29/2023 10:49:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =======================
-- Author:		Rakib
-- Create date: 26-05-2023
-- Description:	<Description,,>
-- Reference: 
-- EXEC [dbo].[SP_OrderItem_GetListByFilter] ''
-- =======================
Create PROCEDURE [dbo].[SP_OrderItem_GetListByFilter]
	@Where NVARCHAR(3000)='',
	@IsCalculateTotal BIT='true',
	@Skip INT = 0,
	@Top INT = 20,
	@SortColumn NVARCHAR(50) = 'oi.[CreatedDate]',
	@SortDirection NVARCHAR(4)='DESC'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE 

	@TotalImageCount DECIMAL(16,0) = 0,
	@SQL NVARCHAR(MAX),
	@FinalSQL NVARCHAR(MAX),		
	@OutPut1 NVARCHAR(max)

	--get totals if page search
	IF(@IsCalculateTotal='true')
	BEGIN

			SET @SQL = N'
			SELECT 
			@TotalImageCount = COUNT(*)
			FROM [dbo].[Order_ClientOrderItem] oi WITH(NOLOCK)
INNER JOIN [dbo].[Order_ClientOrder] o WITH(NOLOCK) on o.Id = oi.ClientOrderId
LEFT JOIN [dbo].[Order_AssignedImageEditor] ed WITH(NOLOCK) on ed.Order_ImageId = oi.Id and ed.IsActive  = 1
LEFT JOIN [dbo].[Security_Contact] edn WITH(NOLOCK) on edn.Id = ed.AssignContactId
left JOIN [dbo].Security_Contact assignby WITH(NOLOCK) ON assignby.Id= ed.AssignByContactId 
	INNER JOIN [dbo].Common_Company c WITH(NOLOCK) ON c.Id = o.CompanyId
	left JOIN dbo.Management_Team T WITH(NOLOCK) ON T.Id=o.AssignedTeamId  '
			+@Where
		
	SET @OutPut1 = N'@TotalImageCount DECIMAL(16,0) OUTPUT ';
	EXEC sp_executesql @SQL, @OutPut1,@TotalImageCount = @TotalImageCount OUTPUT;
	END	
	--select possible columns
	EXECUTE 
	('SELECT  '	

	+@TotalImageCount +' TotalImageCount 
	
	   ,o.[Id] OrderId
      ,o.[CompanyId]
	  ,c.[ObjectId] CompanyObjectId
	  ,c.[Name] CompanyName
      ,o.[OrderNumber]
      ,o.[OrderPlaceDate]
      ,o.[ObjectId] OrderObjectId
	  ,dateadd(d, datediff(d,0, o.[OrderPlaceDate]), 0) OrderPlaceDateOnly
	  ,assignby.[FirstName] AssignByName
	  ,T.[Name] TeamName
	  ,o.[AssignedDateToTeam] TeamAssignedDate
	  ,oi.[Id] ClientOrderItemId
      ,[FileName]
      ,oi.[ExteranlFileInputPath]
      ,oi.[ExternalFileOutputPath]
      ,oi.[InternalFileInputPath]
      ,oi.[InternalFileOutputPath]
      ,oi.[Status]
      --,[IsDeleted]
      ,oi.[CreatedDate]
      ,oi.[CreatedByContactId]
      ,oi.[ObjectId]
      ,oi.[FileSize]
      ,oi.[DistributedTime]
      ,oi.[DistributedByContactId]
      ,oi.[DeadlineTime]
      ,oi.[InProductionTime]
      ,oi.[ProductionDoneTime]
      ,oi.[InQcTime]
      ,oi.[QcByContactId]
      ,oi.[RejectCount]
      ,oi.[TeamId]
      ,oi.[ExternalStatus]
      ,oi.[ProductionDoneFilePath]
      ,oi.[PartialPath]
      ,oi.[FileNameWithoutExtension]
      ,oi.[FileGroup]
      ,oi.[IsExtraOutPutFile]
      ,oi.[ArrivalTime]
	  ,oi.[ClientOrderId]
	  ,oi.[Id]
	  ,edn.[FirstName] EditorName
      ,ed.[AssignContactId] 
	  ,ed.[AssignByContactId]
      ,ed.[AssignDate] AssignToEditorDate
FROM [dbo].[Order_ClientOrderItem] oi WITH(NOLOCK)
INNER JOIN [dbo].[Order_ClientOrder] o WITH(NOLOCK) on o.Id = oi.ClientOrderId
LEFT JOIN [dbo].[Order_AssignedImageEditor] ed WITH(NOLOCK) on ed.Order_ImageId = oi.Id and ed.IsActive  = 1
LEFT JOIN [dbo].[Security_Contact] edn WITH(NOLOCK) on edn.Id = ed.AssignContactId
left JOIN [dbo].Security_Contact assignby WITH(NOLOCK) ON assignby.Id= ed.AssignByContactId  
	INNER JOIN [dbo].Common_Company c WITH(NOLOCK) ON c.Id = o.CompanyId
	left JOIN dbo.Management_Team T WITH(NOLOCK) ON T.Id=o.AssignedTeamId 
	 '
	+@Where
	
	+' ORDER BY '+@SortColumn +' '+ @SortDirection+' '
	+'OFFSET '+ @Skip+' ROWS '
	+'FETCH NEXT '+@Top+' ROWS ONLY' 
	)
END

