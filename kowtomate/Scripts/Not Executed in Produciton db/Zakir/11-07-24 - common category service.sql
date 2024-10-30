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
-- Author:		Md Zakir Hossain
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE sp_common_category_getServices_by_CategoryId
	-- Add the parameters for the stored procedure here
	@categoryId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
	SELECT cs.* FROM Common_CategoryService ccs

	INNER JOIN Common_Category cc on cc.Id = ccs.CommonCategoryId
	INNER JOIN Common_Service cs on cs.Id = ccs.CommonServiceId
	WHERE ccs.CommonCategoryId = @categoryId

END
GO

exec sp_common_category_getServices_by_CategoryId '30'
