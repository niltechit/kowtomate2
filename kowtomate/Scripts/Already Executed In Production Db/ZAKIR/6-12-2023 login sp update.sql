USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Security_User_GetByUsername]    Script Date: 12/6/2023 3:04:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Aminul
-- Create date: 11 Jan 2021
-- Description:	Get All Users
-- =============================================
ALTER PROCEDURE [dbo].[SP_Security_User_GetByUsername]	
@Username NVARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT sc.[Id]
      ,sc.[ContactId]
	  ,sc.[CompanyId]
      ,sc.[Username]
      ,sc.[ProfileImageUrl]
      ,sc.[PasswordHash]
      ,sc.[PasswordSalt]
      ,sc.[RegistrationToken]
      ,sc.[PasswordResetToken]
      ,sc.[Status]
      ,sc.[LastLoginDateUtc]
      ,sc.[LastLogoutDateUtc]
      ,sc.[LastPasswordChangeUtc]
      ,sc.[CreatedDate]
      ,sc.[CreatedByContactId]
	  ,sc.[UpdatedDate]
	  ,sc.[UpdatedByContactId]
      ,sc.[ObjectId]
	  ,su.DownloadFolderPath
  FROM [dbo].[Security_User] sc WITH(NOLOCK) 
  LEFT JOIN Security_Contact su on su.Id=sc.ContactId
  WHERE sc.[Username] = @UserName

END






