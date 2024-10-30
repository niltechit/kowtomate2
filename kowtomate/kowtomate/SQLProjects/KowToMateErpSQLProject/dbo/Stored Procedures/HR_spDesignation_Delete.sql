

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================

CREATE PROCEDURE [dbo].[HR_spDesignation_Delete](
            @DesignationId  int
        
)
AS
BEGIN  
    DELETE FROM  [dbo].[Security_Country] WHERE Id = @DesignationId
END



