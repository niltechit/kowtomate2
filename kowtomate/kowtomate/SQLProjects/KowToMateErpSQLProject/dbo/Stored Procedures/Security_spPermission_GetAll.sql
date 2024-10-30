
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

Create PROCEDURE [dbo].[Security_spPermission_GetAll]

AS
BEGIN  
SELECT [Id]
      ,[Name]
	  ,[Value]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_Permission]

END



