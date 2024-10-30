﻿-- =============================================
-- Author:		Aminul
-- Create date: 11 Jan 2021
-- Description:	Get All Users
-- =============================================
CREATE PROCEDURE [dbo].[spUser_GetByUsername]	
@Username NVARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [Id]
      ,[ContactId]
      ,[RoleId]
      ,[Username]
      ,[ProfileImageUrl]
      ,[PasswordHash]
      ,[PasswordSalt]
      ,[RegistrationToken]
      ,[PasswordResetToken]
      ,[Status]
      ,[LastLoginDateUtc]
      ,[LastLogoutDateUtc]
      ,[LastPasswordChangeUtc]
      ,[CreateFromUserIp]
      ,[CreatedDateUtc]
      ,[ChangedDateUtc]
      ,[UserGuid]
  FROM [dbo].[User] WITH(NOLOCK) WHERE [Username] = @UserName

END


