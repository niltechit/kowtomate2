USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetItemByCompanyIdAndFileNameAndFilePath]    Script Date: 1/2/2023 6:40:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetItemByCompanyIdAndFileNameAndFilePath]
	(
		@ClientOrderId int,
		@FileNameWithoutExtension nvarchar(200),
		@PartialPath nvarchar(250),
		@CompanyId int
	)
AS
BEGIN
   
   SET NOCOUNT ON;

   select * INTO #TempItems1 From 
   Order_ClientOrderItem 
   where ClientOrderId=@ClientOrderId 
   and FileNameWithoutExtension=@FileNameWithoutExtension 
   and PartialPath=@PartialPath 
   and CompanyId=@CompanyId

   if ( (select COUNT(*) FROM #TempItems1) = 0)
   BEGIN
        DECLARE @PartialNewPath varchar(1000), @OrderNumber varchar(50)
	    Select @OrderNumber OrderNumber FROM  [dbo].[Order_ClientOrder] WITH(NOLOCK) WHERE Id = @ClientOrderId 
        
		SET @OrderNumber = '/' + @OrderNumber + ''

		SET @PartialNewPath = Replace(@PartialPath, @OrderNumber, '')
		
		select *INTO #TempItems2 From 
		Order_ClientOrderItem 
		where ClientOrderId=@ClientOrderId 
		and FileNameWithoutExtension=@FileNameWithoutExtension 
		and PartialPath LIKE '%'+  @PartialNewPath 
		and CompanyId=@CompanyId

		 if ( (select COUNT(*) FROM #TempItems2) > 1)
		 BEGIN
		     select * FROM #TempItems2 
		 END
		 ELSE
		 BEGIN
		     return
		 END
   END
   ELSE
   BEGIN
       SELECT *FROM #TempItems1
   END
END
