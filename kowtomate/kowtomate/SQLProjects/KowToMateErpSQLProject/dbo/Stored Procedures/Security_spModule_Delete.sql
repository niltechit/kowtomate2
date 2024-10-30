

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_spModule_Delete](
            @ModuleId  int
        
)
AS
BEGIN  
    DELETE FROM  [dbo].[Security_Module] WHERE Id = @ModuleId
END



