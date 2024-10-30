

-- =============================================
-- Author:		Shabuj Hossain 
-- Create date: 14 Jan 2021
-- Description:	Get All Role data
-- =============================================
Create PROCEDURE [dbo].[spRole_GetAll]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [Id]
      ,[Name]
      ,[ChangedDateUtc]
      ,[RoleGuid]
  FROM [dbo].[Role] WITH(NOLOCK)

END