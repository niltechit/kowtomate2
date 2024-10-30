ALTER TABLE Security_Contact
ADD DownloadFolderPath varchar(500);

GO
/****** Object:  StoredProcedure [dbo].[pdate Contact Folder Path]    Script Date: 1/9/2023 10:09:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Rakib
-- Create date: 1 Jan 2023
-- Description:	Update Contact Folder Path
-- =============================================

Create PROCEDURE [dbo].[SP_User_UpdateUserDownloadFolderPath](
       @DownloadFolderPath varchar(500),
	   @Id int
)
AS
BEGIN  
  UPDATE [dbo].[Security_Contact]    
   SET 
	   [DownloadFolderPath] = @DownloadFolderPath
       WHERE Id = @Id
END


USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetById]    Script Date: 1/9/2023 10:39:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

ALTER PROCEDURE [dbo].[SP_Security_Contact_GetById]
@ContactId int
AS
BEGIN  
	SELECT [Id]
      ,[CompanyId]
      ,[FirstName]
      ,[LastName]
      ,[DesignationId]
      ,[Email]
      ,[Phone]
      ,[ProfileImageUrl]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
	  , [DownloadFolderPath] 
  FROM [dbo].[Security_Contact] Where Id= @ContactId
END
