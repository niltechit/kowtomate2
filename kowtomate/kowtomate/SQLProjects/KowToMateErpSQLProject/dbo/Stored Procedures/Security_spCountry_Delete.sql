

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_spCountry_Delete](
            @CountryId  int
        
)
AS
BEGIN  
    DELETE FROM  [dbo].[Security_Country] WHERE Id = @CountryId
END



