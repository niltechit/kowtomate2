-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[sp_Company_Insert](
            @Name nvarchar(100),
            @Email nvarchar(50),
            @Phone nvarchar(20),
			@FileRootFolderPath nvarchar(100),
            @Status int,
            @CreatedByContactId int,
            @CreatedDateUtc datetime
            
)
AS
BEGIN  
    INSERT INTO [dbo].[Company]
           ([Name]
           ,[Email]
           ,[Phone]
		   ,[FileRootFolderPath]
           ,[Status]
           ,[CreatedByContactId]
           ,[CreatedDateUtc]
           
           )
     VALUES
          (
		   @Name,
           @Email, 
           @Phone,
		   @FileRootFolderPath,
           @Status,
           @CreatedByContactId,
           @CreatedDateUtc
          
		   )
END