USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Service_GetAll]    Script Date: 1/11/2024 5:25:47 PM ******/
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
	SELECT *, Id as CommonServiceReturnId FROM Common_Service with(nolock) where IsActive = 1 and IsDeleted = 0;
END