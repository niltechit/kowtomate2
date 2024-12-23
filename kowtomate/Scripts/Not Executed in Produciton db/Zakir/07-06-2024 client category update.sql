-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
alter PROCEDURE sp_clientCategoryService_getByCategoryId
	-- Add the parameters for the stored procedure here
	@categoryId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	--SELECT CCS.*

	--FROM [dbo].Client_Category CC
	--LEFT JOIN Common_Category CCAT ON CCAT.Id = CC.CommonCategoryId 
	--LEFT JOIN Common_CategoryService CCS ON CCS.CommonCategoryId= CCAT.Id
	--LEFT JOIN Common_Service CS ON CS.Id = CCS.CommonServiceId
	--WHERE CC.CommonCategoryId = @categoryId

	SELECT CS.Name,CS.Id, CCS.PriceInUSD,CCS.TimeInMinutes

	FROM  Common_Category CCAT
	LEFT JOIN Common_CategoryService CCS ON CCS.CommonCategoryId= CCAT.Id
	LEFT JOIN Common_Service CS ON CS.Id = CCS.CommonServiceId
	WHERE CCS.CommonCategoryId = @categoryId
END
GO
exec sp_clientCategoryService_getByCategoryId 23
