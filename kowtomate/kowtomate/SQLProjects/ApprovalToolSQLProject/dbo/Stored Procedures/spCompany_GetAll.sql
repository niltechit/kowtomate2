

-- =============================================
-- Author:		Shabuj Hossain 
-- Create date: 14 Jan 2021
-- Description:	Get All company data
-- =============================================
CREATE PROCEDURE [dbo].[spCompany_GetAll]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT  [Id]
      ,[Name]
      ,[Email]
      ,[Phone]
      ,[Status]
      ,[CreatedByContactId]
      ,[CreatedDateUtc]
      ,[ChangedDateUtc]
	  ,[FileRootFolderPath]
  FROM [dbo].[Company] WITH(NOLOCK)

END


