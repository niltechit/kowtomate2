

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_sp_Role_Insert]( 
           @Name varchar(100),
           @Status tinyint,
           @CreatedByContactId int,
           @ObjectId int
)
AS
BEGIN  
INSERT INTO [dbo].[Security_Role]
           ([Name]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[ObjectId])
     VALUES
           (
		   @Name,
           @Status, 
           SYSDATETIME(), 
           @CreatedByContactId, 
           @ObjectId
		   )
END


