﻿
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_spMenu_Delete](
            @MenuId  int
        
)
AS
BEGIN  
    DELETE FROM  [dbo].[Security_Menu] WHERE Id = @MenuId
END



