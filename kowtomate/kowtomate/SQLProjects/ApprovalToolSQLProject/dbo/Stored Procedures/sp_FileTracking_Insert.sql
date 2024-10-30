-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save file info of Accepted/rejected images
-- =============================================

CREATE PROCEDURE [dbo].[sp_FileTracking_Insert](
    @CompanyId AS INT,
	@SourceDrive AS NVARCHAR(30),
	@ParentDirectory AS NVARCHAR(250),
	@BucketName AS NVARCHAR(50),
	@ActionDate AS DATETIME,
	@ActionType AS NVARCHAR(20),
	@Attachment AS NVARCHAR(250),
	@Comments AS NVARCHAR(MAX),
	@MarkupImageUrl AS NVARCHAR(500),
    @NoOfFiles as int,
    @SelectedFileNames AS NVARCHAR(MAX),
	@CreatedByContactId AS INT,
	@CreatedDateUtc AS DATETIME,
	@Status AS INT
)
AS
BEGIN
	  BEGIN TRY
           INSERT INTO [dbo].[FileTracking]
           ([CompanyId]
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
           ,[Status])
     VALUES
           (@CompanyId
           ,@SourceDrive
           ,@ParentDirectory
           ,@BucketName
           ,@ActionDate
           ,@ActionType
           ,@Attachment
           ,@Comments
		   ,@MarkupImageUrl
		   ,@NoOfFiles
		   ,@SelectedFileNames
           ,@CreatedByContactId
           ,@CreatedDateUtc
           ,@Status)

	  END TRY
	  BEGIN CATCH
	  END CATCH
END

