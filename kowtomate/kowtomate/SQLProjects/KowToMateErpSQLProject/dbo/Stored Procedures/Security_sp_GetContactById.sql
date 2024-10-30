
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_sp_GetContactById]
@ContactId int

AS
BEGIN  
	SELECT [Id]
      ,[CompanyId]
      ,[FirstName]
      ,[LastName]
      ,[Designation]
      ,[Email]
      ,[Phone]
      ,[ProfileImageUrl]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_Contact] Where Id= @ContactId
END



