
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_spContact_Delete](
            @ContactId  int
        
)
AS
BEGIN  
    DELETE FROM  [dbo].[Security_Contact] WHERE Id = @ContactId
END



