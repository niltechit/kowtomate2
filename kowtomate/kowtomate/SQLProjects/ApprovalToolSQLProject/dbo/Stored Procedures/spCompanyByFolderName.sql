
-- =============================================
-- Author:		Shabuj
-- Create date: 15 Jan 2021
-- Description:	Get Company by Folder Name
-- Exec: [dbo].[spCompanyByFolderName] 'Client 1'
-- =============================================
CREATE PROCEDURE [dbo].[spCompanyByFolderName]
@FolderName nvarchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [Id]
      ,[Name]
      ,[Email]
      ,[Phone]
      ,[Status]
      ,[CreatedByContactId]
      ,[CreatedDateUtc]
      ,[ChangedDateUtc]
      ,[FileRootFolderPath]
  FROM [dbo].[Company] WITH(NOLOCK) 
  WHERE FileRootFolderPath=@FolderName
 END