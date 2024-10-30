Go
alter table Order_ClientOrder
add  AllowExtraOutputFileUpload bit

Go
alter table Order_ClientOrder add constraint df_AllowExtraOutputFileUpload default 0 for AllowExtraOutputFileUpload
Go
update Order_ClientOrder  set AllowExtraOutputFileUpload = 0 where AllowExtraOutputFileUpload = null 

GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrder_AllowExtraOutputFileUploadFieldUpdate]    Script Date: 12/27/2022 2:37:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Rakib
-- Create date: 27 Dec 2022
-- Description:	Update Order  [SP_Order_ClientOrder_AllowExtraOutputFileUploadFieldUpdate]
-- =============================================

Create PROCEDURE [dbo].[SP_Order_ClientOrder_AllowExtraOutputFileUploadFieldUpdate]
(
	@Id bigint,
	@AllowExtraOutputFileUpload bit
	
)
as 
begin 
	update Order_ClientOrder
	set
	AllowExtraOutputFileUpload=@AllowExtraOutputFileUpload
	where Id=@Id
end


GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetItemByCompanyIdAndFileName]    Script Date: 12/27/2022 6:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rakib
-- Create date: 27 Dec 2022
-- Description:	Find Order Item [SP_Order_ClientOrderItem_GetItemByCompanyIdAndFileName]
-- =============================================
Create PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetItemByCompanyIdAndFileName]
	(
		@ClientOrderId int,
		@FileNameWithoutExtension nvarchar(200),
		@CompanyId int
	)
AS
BEGIN
   
   SET NOCOUNT ON;

   select * From 
   Order_ClientOrderItem 
   where ClientOrderId=@ClientOrderId 
   and FileNameWithoutExtension=@FileNameWithoutExtension 
   and CompanyId=@CompanyId

END




GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrder_GetListByFilter]    Script Date: 12/28/2022 5:23:10 PM ******/
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
	@SQL AS NVARCHAR(MAX),
	@FinalSQL AS NVARCHAR(MAX),		
	@OutPut1 NVARCHAR(max)

	--get totals if page search
	IF(@IsCalculateTotal='true')
	BEGIN

			SET @SQL = N'
			SELECT 
			@TotalCount =COUNT(*)
			FROM [dbo].[Order_ClientOrder] o WITH(NOLOCK) 
	left JOIN [dbo].Security_Contact assignby WITH(NOLOCK) ON assignby.Id=o.[AssignedByOpsContactId] 
	Left JOIN [dbo].Common_Company c WITH(NOLOCK) ON c.Id = o.CompanyId
	left JOIN dbo.Management_Team T WITH(NOLOCK) ON T.Id=o.AssignedTeamId '
			+@Where
		
	SET @OutPut1 = N'@TotalCount INT OUTPUT ';
	EXEC sp_executesql @SQL, @OutPut1, @TotalCount =@TotalCount OUTPUT; 
	END	
	--select possible columns
	EXECUTE 
	('SELECT  '	
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
	  ,o.[ExternalOrderStatus]
	  ,o.AllowExtraOutputFileUpload
	  ,dateadd(d, datediff(d,0, o.[OrderPlaceDate]), 0) OrderPlaceDateOnly
	  ,assignby.[FirstName] ContactName
	  ,T.[Name] TeamName
	  ,o.[AssignedDateToTeam] TeamAssignedDate
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
