
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[HR_sp_GetDesignationById]
@DesignationId int
AS
BEGIN  
	SELECT  [Id]
      ,[Name]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[HR_Designation] WHERE Id = @DesignationId
END



