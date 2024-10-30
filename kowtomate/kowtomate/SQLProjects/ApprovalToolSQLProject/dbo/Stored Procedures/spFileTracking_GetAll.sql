
-- =============================================
-- Author:		Shabuj Hossain 
-- Create date: 14 Jan 2021
-- Description:	Get File tracking data
-- =============================================
CREATE PROCEDURE [dbo].[spFileTracking_GetAll]	
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
      ,[CreatedByContactId]
      ,[CreatedDateUtc]
      ,[Status]
  FROM [dbo].[FileTracking] WITH(NOLOCK)

END


