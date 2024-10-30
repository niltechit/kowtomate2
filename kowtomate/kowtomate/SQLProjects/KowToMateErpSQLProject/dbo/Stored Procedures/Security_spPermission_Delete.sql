

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_spPermission_Delete](
            @PermissionId  int
        
)
AS
BEGIN  
    DELETE FROM  [dbo].[Security_Permission] WHERE Id = @PermissionId
END



