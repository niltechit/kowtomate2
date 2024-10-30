

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_sp_Role_Update](
           @Id  int,
           @Name varchar(100),
           @Status tinyint,
           @UpdatedByContactId int
)
AS
BEGIN  
   UPDATE [dbo].[Security_Role]
   SET 
      [Name] = @Name, 
      [Status] = @Status,
      [UpdatedDate] = SYSDATETIME(),
      [UpdatedByContactId] = @UpdatedByContactId
   
		
     WHERE Id = @Id
END



