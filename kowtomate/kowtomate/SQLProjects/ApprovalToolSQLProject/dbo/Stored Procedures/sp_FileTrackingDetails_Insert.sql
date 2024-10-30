-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save file info of Accepted/rejected images
-- =============================================

CREATE PROCEDURE [dbo].[sp_FileTrackingDetails_Insert](
     @FileTrackingId int,
     @FileName nvarchar(200),
     @FilePathUrl nvarchar(max),
     @FileType nvarchar(30),
     @FileSize bigint,
     @FileMarkUp nvarchar(100)
    
)
AS
BEGIN
	  BEGIN TRY
           INSERT INTO [dbo].[FileTrackingDetails]
           (
		    [FileTrackingId]
		   ,[FileName]
           ,[FilePathUrl]
           ,[FileType]
           ,[FileSize]
           ,[FileMarkUp]
		   )    
     VALUES
           (@FileTrackingId
		   ,@FileName
           ,@FilePathUrl
           ,@FileType
           ,@FileSize
           ,@FileMarkUp
           
           )

	  END TRY
	  BEGIN CATCH
	  END CATCH
END

