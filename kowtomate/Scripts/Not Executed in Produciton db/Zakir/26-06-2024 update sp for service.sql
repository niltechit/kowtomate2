USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Service_GetAll]    Script Date: 6/24/2024 2:50:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_Common_Service_GetAll] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
		* 
	FROM 
		Common_Service with(nolock) 
	WHERE 
		 IsDeleted = 0;
END
