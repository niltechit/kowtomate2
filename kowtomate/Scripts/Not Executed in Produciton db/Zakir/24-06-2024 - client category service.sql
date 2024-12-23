USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Client_CategoryService_GetByServiceIdAndCategoryId]    Script Date: 6/24/2024 6:50:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Get by id common category
-- =============================================
ALTER PROCEDURE [dbo].[SP_Client_CategoryService_GetByServiceIdAndCategoryId] 
	-- Add the parameters for the stored procedure here
	@ServiceId int,
	@CategoryId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM Client_CategoryService with(nolock) WHERE CommonServiceId = @ServiceId and ClientCategoryId = @CategoryId
END
