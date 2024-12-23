USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_client_category_getCategoryServicesByCategoryId]    Script Date: 6/24/2024 8:47:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_client_category_getCategoryServicesByCategoryId]
	-- Add the parameters for the stored procedure here
	@categoryId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
	SELECT 
	ccs.Id,
	cs.Name, 
	--ccs.CommonServiceId as Id,
	ccs.ClientCategoryId,
	ccs.TimeInMinutes,
	ccs.PriceInUSD
	FROM Client_CategoryService ccs
	INNER JOIN Common_Service cs on cs.Id = ccs.CommonServiceId
	WHERE ccs.ClientCategoryId = @categoryId and ccs.IsDeleted = 1

END
