
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[HR_sp_Designation_Update](
            @Id  int,
            @Name  nvarchar(100),
            @Status int,
            @UpdatedByContactId int
)
AS
BEGIN  
    UPDATE [dbo].[HR_Designation]
      SET 
	  [Name] = @Name,
      [Status] = @Status,    
      [UpdatedDate] = SYSDATETIME(),
      [UpdatedByContactId] = @UpdatedByContactId

      WHERE Id = @Id
END



