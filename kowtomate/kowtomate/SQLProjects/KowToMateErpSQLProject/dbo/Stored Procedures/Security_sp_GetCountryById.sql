
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_sp_GetCountryById]
@CountryId int

AS
BEGIN  
	SELECT  [Id]
      ,[Name]
      ,[Code]
      ,[ObjectId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
  FROM [dbo].[Security_Country] Where Id= @CountryId
END



