
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_sp_GetMenuById]
@MenuId int
AS
BEGIN  
SELECT [Id]
      ,[Name]
      ,[ParentId]
      ,[Icon]
      ,[IsLeftMenu]
      ,[IsTopMenu]
      ,[IsExternalMenu]
      ,[MenuUrl]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_Menu] WHERE Id = @MenuId

END



