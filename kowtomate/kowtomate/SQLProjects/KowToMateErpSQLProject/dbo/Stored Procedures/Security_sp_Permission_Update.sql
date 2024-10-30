

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_sp_Permission_Update](
           @Id  int,
           @Name varchar(100),
		   @Value varchar(100),
           @Status tinyint,
           @UpdatedByContactId int
)
AS
BEGIN  
   UPDATE [dbo].[Security_Permission]
   SET 
      [Name] = @Name, 
	  [Value] = @Value,
      [Status] = @Status,
      [UpdatedDate] = SYSDATETIME(),
      [UpdatedByContactId] = @UpdatedByContactId
   
		
     WHERE Id = @Id
END



