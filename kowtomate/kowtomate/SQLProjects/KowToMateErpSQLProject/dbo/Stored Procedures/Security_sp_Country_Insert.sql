
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_sp_Country_Insert](
       @Name varchar(100),
       @Code varchar(6),
	   @ObjectId varchar(40),
       @CreatedByContactId int
)
AS
BEGIN  
  INSERT INTO [dbo].[Security_Country]
           ([Name]
           ,[Code]
           ,[ObjectId]
           ,[CreatedDate]
           ,[CreatedByContactId]
          )
     VALUES
           (
		   @Name,
           @Code,
           @ObjectId,
           SYSDATETIME(), 
           @CreatedByContactId
		   )

END



