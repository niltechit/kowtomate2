
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_sp_Country_Update](
       @Id int,
       @Name varchar(100),
       @Code varchar(6),
       @UpdatedByContactId int
)
AS
BEGIN  
  UPDATE [dbo].[Security_Country]    
   SET 
	   [Name] = @Name,
       [Code] = @Code, 
       [UpdatedDate] = SYSDATETIME(),
       [UpdatedByContactId] = @UpdatedByContactId
       WHERE Id = @Id
END



