
create PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetItemByCompanyIdAndFullFileNameAndFilePath]
	(
		@ClientOrderId int,
		@FileName nvarchar(200),
		@PartialPath nvarchar(250),
		@CompanyId int
	)
AS
BEGIN
   
   SET NOCOUNT ON;

   select * INTO #TempItems1 From 
   Order_ClientOrderItem 
   where ClientOrderId=@ClientOrderId 
   and FileName=@FileName 
   and PartialPath=@PartialPath 
   and CompanyId=@CompanyId

   if ( (select COUNT(*) FROM #TempItems1) = 0)
   BEGIN
        DECLARE @PartialNewPath varchar(1000), @OrderNumber varchar(50)
	    Select @OrderNumber OrderNumber FROM  [dbo].[Order_ClientOrder] WITH(NOLOCK) WHERE Id = @ClientOrderId 
        
		SET @OrderNumber = '/' + @OrderNumber + ''

		SET @PartialNewPath = Replace(@PartialPath, @OrderNumber, '')
		
		select *INTO #TempItems2 From 
		Order_ClientOrderItem WITH(NOLOCK)
		where ClientOrderId=@ClientOrderId 
		and FileName=@FileName 
		and PartialPath LIKE '%'+  @PartialNewPath 
		and CompanyId=@CompanyId
		-- Added Zakir --- 
		and [FileGroup]= 1  -- 1 Means workable file
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
