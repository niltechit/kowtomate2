

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_spRole_Delete](
            @RoleId  int
        
)
AS
BEGIN  
    DELETE FROM  [dbo].[Security_Role] WHERE Id = @RoleId
END



