
-- =============================================
-- Author:		Aminul
-- Create date: 15 Jan 2021
-- Description:	Get Issues by Company Id
-- Exec: [dbo].[spFileTracking_GetIssuesByCompanyId] 0,'Rejected'
-- =============================================
CREATE PROCEDURE [dbo].[spFileTracking_GetIssuesByCompanyId]
@CompanyId INT,
@ActionType NVARCHAR(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
	   [Id]
      ,[CompanyId]
      ,[SourceDrive]
      ,[ParentDirectory]
      ,[BucketName]
      ,[ActionDate]
      ,[ActionType]
      ,[Attachment]
      ,[Comments]
	  ,[MarkupImageUrl]
	  ,[NoOfFiles]
	  ,[SelectedFileNames]
      ,[CreatedByContactId]
      ,[CreatedDateUtc]
      ,[Status]
  FROM [dbo].[FileTracking] WITH(NOLOCK) 
  WHERE (@CompanyId = 0 OR CompanyId = @CompanyId)
  AND ActionType = @ActionType
 END