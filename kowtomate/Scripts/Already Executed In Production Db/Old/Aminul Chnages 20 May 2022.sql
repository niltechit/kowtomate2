-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Md Aminul Islam
-- Create date: 20 May 2022
-- Description:	Get Permisison by User Id
-- =============================================
CREATE PROCEDURE [dbo].[SP_Security_Permission_GetAllByUserId]
@UserId INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    -- Insert statements for procedure here
	SELECT DISTINCT *FROM [dbo].[Security_Permission] p WITH(NOLOCK)
	WHERE p.Id IN (SELECT rp.PermissionId FROM [dbo].[Security_RolePermission] rp WITH(NOLOCK) 
	WHERE RoleId IN (SELECT ur.RoleId FROM [dbo].[Security_UserRole] ur WITH(NOLOCK) WHERE ur.UserId = @UserId))
END
GO

ALTER TABLE [dbo].[Security_User]
DROP Constraint FK_User_Role

GO
ALTER TABLE [dbo].[Security_User]
DROP COLUMN [RoleId]

GO
/****** Object:  StoredProcedure [dbo].[SP_Security_User_GetByUsername]    Script Date: 5/20/2022 4:57:55 PM ******/
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
	SELECT [Id]
      ,[ContactId]
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
      ,[CreatedDate]
      ,[CreatedByContactId]
	  ,[UpdatedDate]
	  ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_User] WITH(NOLOCK) WHERE [Username] = @UserName

END

GO
ALTER TABLE [dbo].[Common_Company]
ALTER COLUMN CreatedByContactId INT NULL

GO
ALTER TABLE [dbo].[Security_Contact]
ALTER COLUMN CreatedByContactId INT NULL

GO
ALTER TABLE [dbo].[Security_Role]
ADD CompanyId INT NOT NULL


ALTER TABLE [dbo].[Security_Role]  WITH CHECK ADD  CONSTRAINT [FK_Role_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Common_Company] ([Id])
GO

ALTER TABLE [dbo].[Security_Role] CHECK CONSTRAINT [FK_Role_Company]
GO

ALTER TABLE [dbo].[Security_Contact]
ALTER COLUMN DesignationId INT

GO
/****** Object:  StoredProcedure [dbo].[SP_Security_User_GetByUsername]    Script Date: 5/20/2022 6:45:01 PM ******/
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
	SELECT [Id]
      ,[ContactId]
	  ,[CompanyId]
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
      ,[CreatedDate]
      ,[CreatedByContactId]
	  ,[UpdatedDate]
	  ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_User] WITH(NOLOCK) WHERE [Username] = @UserName

END