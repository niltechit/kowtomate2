
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Common_spCompany_Delete](
            @CompanyId  int
        
)
AS
BEGIN  
    DELETE FROM  [dbo].[Common_Company] WHERE Id = @CompanyId
END



