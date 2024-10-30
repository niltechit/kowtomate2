GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetAllByTeamId]    Script Date: 4/28/2023 6:59:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Rakib	
-- Create date: 28-04-2023
-- Description:	Get Contact By Team Id

-- =============================================
Create PROCEDURE [dbo].[SP_Security_Contact_GetAllByTeamId]
(
	@TeamId int
)
AS
BEGIN  
	SELECT c.CompanyId,
	c.FirstName,
	c.LastName,
	c.DesignationId,
	c.Email,
	c.Phone,
	c.Phone,
	c.ProfileImageUrl,
	c.Status,
	c.CreatedDate,
	c.CreatedByContactId,
	c.UpdatedByContactId,
	c.ObjectId,
	c.EmployeeId,
	c.IsEmployee,
	c.DownloadFolderPath,
	c.IsUserActive,
	c.isSharedFolderEnable
  FROM [dbo].[Security_Contact] c WITH(NOLOCK)
  INNER JOIN dbo.Management_TeamMember as mt on mt.ContactId = c.Id
  where mt.TeamId = @TeamId
  
 
END