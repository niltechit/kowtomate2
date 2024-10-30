

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_sp_Permission_Insert]( 
           @Name varchar(100),
		   @Value varchar(100),
           @Status tinyint,
           @CreatedByContactId int,
           @ObjectId int
)
AS
BEGIN  
INSERT INTO [dbo].[Security_Permission]
           ([Name]
		   ,[Value]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[ObjectId])
     VALUES
           (
		   @Name,
		   @Value,
           @Status, 
           SYSDATETIME(), 
           @CreatedByContactId, 
           @ObjectId
		   )
END


