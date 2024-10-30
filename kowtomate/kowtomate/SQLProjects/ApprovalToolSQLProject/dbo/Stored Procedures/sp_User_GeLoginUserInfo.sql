-- =============================================
-- Author:		Aminul
-- Create date: 11 Jan 2021
-- Description:	Get Login User Info 
-- Exec: [dbo].[sp_User_GeLoginUserInfo] 'superadmin'
-- =============================================
CREATE PROCEDURE [dbo].[sp_User_GeLoginUserInfo]	
@UserGuid VARCHAR(32)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
	    u.Id UserId,
		u.ContactId,
		u.RoleId,
		r.[Name] RoleName,
		u.Username,
		u.ProfileImageUrl,
		u.Status,
		u.LastLoginDateUtc,
		u.UserGuid,
		con.FirstName,
		con.LastName,
		con.Email,
		com.Name CompanyName,
		con.CompanyId,
		com.FileRootFolderPath ClientRootFolderPath
  FROM [dbo].[User] u 
  INNER JOIN [dbo].[Role] r on r.Id = u.RoleId
  INNER JOIN [dbo].[Contact] con on con.Id = u.ContactId
  INNER JOIN [dbo].[Company] com on com.Id = con.CompanyId
  WHERE u.UserGuid = @UserGuid
END


