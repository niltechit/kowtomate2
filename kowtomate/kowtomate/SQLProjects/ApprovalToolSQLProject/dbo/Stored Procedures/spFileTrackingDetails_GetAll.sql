
-- =============================================
-- Author:		Shabuj Hossain 
-- Create date: 14 Jan 2021
-- Description:	Get File tracking details data
-- =============================================
CREATE PROCEDURE [dbo].[spFileTrackingDetails_GetAll]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT
	   [Id]
      ,[FileTrackingId]
      ,[FileName]
      ,[FilePathUrl]
      ,[FileType]
      ,[FileSize]
      ,[FileMarkUp]
  FROM [dbo].[FileTrackingDetails] WITH(NOLOCK)

END


