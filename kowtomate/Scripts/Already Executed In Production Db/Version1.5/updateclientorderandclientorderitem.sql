USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_insert]    Script Date: 5/9/2023 2:39:31 PM ******/
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
	@FileGroup int,
	@IsExtraOutPutFile bit,
	@ArrivalTime datetime
)
as
begin
	insert into 
	Order_ClientOrderItem([FileName],ClientOrderId,[Status],IsDeleted, CreatedDate,UpdatedDate,ObjectId,FileSize,ExternalStatus,FileByteString,InternalFileOutputPath,InternalFileInputPath,ExternalFileOutputPath,CompanyId,PartialPath,FileNameWithoutExtension,[FileGroup],IsExtraOutPutFile,ArrivalTime)
					  
	values(@FileName,@ClientOrderId,@Status,@IsDeleted, @CreatedDate,@UpdatedDate,@ObjectId,@FileSize,@ExternalStatus,@FileByteString,@InternalFileOutputPath,@InternalFileInputPath,@ExternalFileOutputPath,@CompanyId,@PartialPath,@FileNameWithoutExtension,@FileGroup,@IsExtraOutPutFile,@ArrivalTime)

    SELECT SCOPE_IDENTITY();
end



SET QUOTED_IDENTIFIER ON
GO


-- Author:		Md Rakib Hossain
-- Create date: 10-11-2022
-- Description:	Client Order Items MinStatusByOrderId
-- =============================================
Create PROCEDURE [dbo].[SP_Order_ClientOrderItemsMinArrivalTimeByOrderId] 
	@OrderId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT  Min(ArrivalTime) as ArrivalTime from [dbo].[Order_ClientOrderItem] where ClientOrderId=@OrderId 
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrder_StatusUpdate]    Script Date: 5/9/2023 3:04:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[SP_Order_ClientOrder_UpdateArrivalTime]
(
	@Id bigint,
	@ArrivalTime datetime
	
	
)
as 
begin 
	update Order_ClientOrder
	set
	ArrivalTime=@ArrivalTime
	
	where Id=@Id
end


GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetAllByOrderId]    Script Date: 5/9/2023 3:51:39 PM ******/
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
  left join dbo.Order_AssignedImageEditor as assignorder With(Nolock) on orderitem.Id = assignorder.Order_ImageId 
  left join dbo.Security_Contact as contact With(Nolock) on contact.Id=assignorder.AssignContactId
  left join dbo.Management_Team as mt With(Nolock) on mt.Id = orderitem.TeamId
  
   where ClientOrderId=@OrderId and (IsDeleted=0 or IsDeleted is null) and orderItem.FileGroup <> 4 --4 means ColorRef


END




GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetAllByOrderIdAndStatus]    Script Date: 5/9/2023 3:52:53 PM ******/
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
  left join dbo.Order_AssignedImageEditor as assignorder With(Nolock) on orderitem.Id = assignorder.Order_ImageId 
  left join dbo.Security_Contact as contact With(Nolock) on contact.Id=assignorder.AssignContactId
  left join dbo.Management_Team as mt With(Nolock) on mt.Id = orderitem.TeamId
  
   where ClientOrderId=@OrderId and (IsDeleted=0 or IsDeleted is null) and orderItem.FileGroup <> 4 and orderItem.ExternalStatus in (24,26)--4 means ColorRef ,,, -- 24 = Ready To Download and 26 = Completed


END



USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderAssignedItem_GetByOrderId]    Script Date: 5/9/2023 3:53:54 PM ******/
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
	inner join Order_AssignedImageEditor as assignorder WITH(NOLOCK) on orderitem.Id = assignorder.Order_ImageId
	inner join dbo.Security_Contact as contact  WITH(NOLOCK) on contact.Id=assignorder.AssignContactId
	left join dbo.Management_Team as mt  WITH(NOLOCK) on mt.Id = orderitem.TeamId
	where orderitem.ClientOrderId=@OrderId and assignorder.AssignContactId = @ContactId and orderitem.FileGroup <> 4 --4 means ColorRef
end






GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetEqualAndGreaterItemByStatus]    Script Date: 5/9/2023 3:54:53 PM ******/
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
	   ,orderitem.ArrivalTime
	from dbo.Order_ClientOrderItem as orderitem 
	left join Order_AssignedImageEditor as assignorder WITH(NOLOCK) on orderitem.Id = assignorder.Order_ImageId
	left join dbo.Security_Contact as contact WITH(NOLOCK) on contact.Id=assignorder.AssignContactId
	left join dbo.Management_Team as mt WITH(NOLOCK) on mt.Id = orderitem.TeamId
	
	 where ClientOrderId = @OrderId and orderitem.Status >= @Status and orderItem.FileGroup <> 4 --4 means ColorRef
end



GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrder_GetAllListByFilter]    Script Date: 5/9/2023 4:34:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =======================
-- Author:		Zakir
-- Create date: 20-09-2022
-- Description:	<Description,,>
-- Reference: 
-- EXEC [dbo].[SP_Order_ClientOrder_GetListByFilter] ''
-- =======================
ALTER PROCEDURE [dbo].[SP_Order_ClientOrder_GetAllListByFilter]
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
	@SQL AS NVARCHAR(MAX),
	@FinalSQL AS NVARCHAR(MAX),		
	@OutPut1 NVARCHAR(max)

	--get totals if page search
	IF(@IsCalculateTotal='true')
	BEGIN

	if(@Where=0)
	begin
	SET @SQL = N'
	SELECT 
	@TotalCount =COUNT(*)
	FROM [dbo].[Order_ClientOrder] o WITH(NOLOCK)'
	end
	else
	begin
	SET @SQL = N'
	SELECT 
	@TotalCount =COUNT(*)
	FROM [dbo].[Order_ClientOrder] o WITH(NOLOCK)'
	+@Where
	end
	SET @OutPut1 = N'@TotalCount INT OUTPUT ';
	EXEC sp_executesql @SQL, @OutPut1, @TotalCount =@TotalCount OUTPUT; 
	END	
	--select possible columns
	EXECUTE 
	('SELECT '	
	+@TotalCount+' TotalCount
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
	  ,o.[ArrivalTime]
	FROM [dbo].[Order_ClientOrder] o WITH(NOLOCK)
	INNER JOIN [dbo].Common_Company c WITH(NOLOCK) ON c.Id = o.CompanyId '
	+@Where
	+' ORDER BY '+@SortColumn +' '+ @SortDirection+' '
	+'OFFSET '+ @Skip+' ROWS '
	+'FETCH NEXT '+@Top+' ROWS ONLY'
	)
END


GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrder_GetAllListByFilterForTeam]    Script Date: 5/9/2023 4:37:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =======================
-- Author:		Zakir
-- Create date: 21-09-2022
-- Description:	<Description,,>
-- Reference: 
-- EXEC [dbo].[SP_Order_ClientOrder_GetListByFilter] ''
-- =======================
ALTER PROCEDURE [dbo].[SP_Order_ClientOrder_GetAllListByFilterForTeam]
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
	@SQL AS NVARCHAR(MAX),
	@FinalSQL AS NVARCHAR(MAX),		
	@OutPut1 NVARCHAR(max)

	--get totals if page search
	IF(@IsCalculateTotal='true')
	BEGIN

	SET @SQL = N'
	SELECT 
	@TotalCount =COUNT(*)
	FROM [dbo].[Order_Assigned_Team] A WITH(NOLOCK)'
	+@Where

	SET @OutPut1 = N'@TotalCount INT OUTPUT ';
	EXEC sp_executesql @SQL, @OutPut1, @TotalCount =@TotalCount OUTPUT; 
	END	
	--select possible columns
	EXECUTE 
	('SELECT '	
	+@TotalCount+' TotalCount
	   ,o.[Id]
      ,o.[CompanyId]
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
	  ,o.[ArrivalTime]
	FROM [dbo].[Order_Assigned_Team] A WITH(NOLOCK) 
	INNER JOIN [dbo].[Order_ClientOrder] o WITH(NOLOCK) on A.OrderId=o.Id
	INNER JOIN [dbo].Management_Team T WITH(NOLOCK) ON T.Id = A.TeamId'
	+@Where
	+' ORDER BY '+@SortColumn +' '+ @SortDirection+' '
	+'OFFSET '+ @Skip+' ROWS '
	+'FETCH NEXT '+@Top+' ROWS ONLY'
	)
END


USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrder_GetListByFilter]    Script Date: 5/9/2023 5:10:50 PM ******/
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










