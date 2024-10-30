
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_spCountry_GetAll]


AS
BEGIN  
	SELECT  [Id]
      ,[Name]
      ,[Code]
      ,[ObjectId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
  FROM [dbo].[Security_Country]
END



