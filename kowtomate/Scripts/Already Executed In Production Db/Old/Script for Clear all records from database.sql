USE [KowToMateERP]

-- clean old data
DELETE FROM [dbo].[Security_Role]
DBCC CHECKIDENT ('[dbo].[Security_Role]', RESEED, 0);

GO
DELETE FROM [dbo].[Security_Contact]
DBCC CHECKIDENT ('[dbo].[Security_Contact]', RESEED, 0);

GO
DELETE FROM [dbo].[Common_Company]
DBCC CHECKIDENT ('[dbo].[Common_Company]', RESEED, 0);

GO
-- Add Super admin (owner company)

IF NOT EXISTS(SELECT 1 FROM  [dbo].[Common_Company] WITH(NOLOCK)
          WHERE Code = 'KTMSYS')
BEGIN
-- Owner Company (type = System)
INSERT INTO [dbo].[Common_Company]
           ([Name]
           ,[Code]
           ,[CompanyType]
           ,[Telephone]
           ,[Email]
           ,[Address1]
           ,[Address2]
           ,[City]
           ,[State]
           ,[Zipcode]
           ,[Country]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     VALUES
           ('Kow  to Mate System' --Name
           ,'KTMSYS' -- [Code]
           ,1 -- [CompanyType]
           ,NULL -- [Telephone]
           ,NULL -- Email
           ,NULL -- Address 1
           ,NULL -- Address2
           ,NULL --City
           ,NULL -- State
           ,NULL --Zipcode
           ,NULL --Country
           ,1 -- Status
           ,GETDATE() --CreatedDate
           ,NULL -- CreatedByContactId
           ,NULL --UpdatedDate
           ,NULL --UpdatedByContactId
           ,LOWER(NEWID()) --ObjectId
		   )

END

GO
IF NOT EXISTS(SELECT 1 FROM  [dbo].[Common_Company] WITH(NOLOCK)
          WHERE Code = 'KTMOP')
BEGIN
-- Owner Company (type = System)
INSERT INTO [dbo].[Common_Company]
           ([Name]
           ,[Code]
           ,[CompanyType]
           ,[Telephone]
           ,[Email]
           ,[Address1]
           ,[Address2]
           ,[City]
           ,[State]
           ,[Zipcode]
           ,[Country]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     VALUES
           ('Kow  to Mate Operation' --Name
           ,'KTMOP' -- [Code]
           ,2 -- [CompanyType]
           ,NULL -- [Telephone]
           ,NULL -- Email
           ,NULL -- Address 1
           ,NULL -- Address2
           ,NULL --City
           ,NULL -- State
           ,NULL --Zipcode
           ,NULL --Country
           ,1 -- Status
           ,GETDATE() --CreatedDate
           ,NULL -- CreatedByContactId
           ,NULL --UpdatedDate
           ,NULL --UpdatedByContactId
           ,LOWER(NEWID()) --ObjectId
		   )

END

GO
GO
IF NOT EXISTS(SELECT 1 FROM  [dbo].[Common_Company] WITH(NOLOCK)
          WHERE Code = 'KTMCLI')
BEGIN
-- Owner Company (type = System)
INSERT INTO [dbo].[Common_Company]
           ([Name]
           ,[Code]
           ,[CompanyType]
           ,[Telephone]
           ,[Email]
           ,[Address1]
           ,[Address2]
           ,[City]
           ,[State]
           ,[Zipcode]
           ,[Country]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     VALUES
           ('Kow  to Mate CLient' --Name
           ,'KTMCLI' -- [Code]
           ,3 -- [CompanyType]
           ,NULL -- [Telephone]
           ,NULL -- Email
           ,NULL -- Address 1
           ,NULL -- Address2
           ,NULL --City
           ,NULL -- State
           ,NULL --Zipcode
           ,NULL --Country
           ,1 -- Status
           ,GETDATE() --CreatedDate
           ,NULL -- CreatedByContactId
           ,NULL --UpdatedDate
           ,NULL --UpdatedByContactId
           ,LOWER(NEWID()) --ObjectId
		   )
END

-- End of adding default companies

-- ====End of adding defualt roles ============
-- Add System Admin user and contact 
IF NOT EXISTS(SELECT 1 FROM  [dbo].[Security_Contact] WITH(NOLOCK)
          WHERE FirstName = 'System' AND LastName = 'Admin')
BEGIN

INSERT INTO [dbo].[Security_Contact]
           ([CompanyId]
           ,[FirstName]
           ,[LastName]
           ,[DesignationId]
           ,[Email]
           ,[Phone]
           ,[ProfileImageUrl]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     
           SELECT 
		   (SELECT TOP 1 Id FROM [dbo].[Common_Company] WHERE Code = 'KTMSYS') as CompanyId   --<CompanyId, int,>
           ,'System'--<FirstName, nvarchar(100),>
           ,'Admin'--<LastName, nvarchar(100),>
           ,NULL --<DesignationId, int,>
           ,NULL --<Email, nvarchar(100),>
           ,NULL --<Phone, varchar(20),>
           ,NULL --<ProfileImageUrl, varchar(200),>
           ,1 -- <Status, int,>
           ,GETDATE() --<CreatedDate, datetime,>
           ,NULL --<CreatedByContactId, int,>
           ,NULL --<UpdatedDate, datetime,>
           ,NULL --<UpdatedByContactId, int,>
           ,LOWER(NEWID()) -- <ObjectId, varchar(40),>)
END
GO
--==================Add Default Contacts & Users ============================

-- ============ Add default roles ==================
IF NOT EXISTS(SELECT 1 FROM  [dbo].[Security_Role] WITH(NOLOCK)
          WHERE [Name] = 'System Admin')
BEGIN
INSERT INTO [dbo].[Security_Role]
           ([Name]
		   ,[CompanyId]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     SELECT 
           'System Admin' --<Name, varchar(100),>
		   ,(SELECT TOP 1 Id FROM [dbo].[Common_Company] WHERE Code = 'KTMSYS') as CompanyId   --<CompanyId, int,>
           ,1 --<Status, tinyint,>
           ,GETDATE() --<CreatedDate, datetime,>
           ,(SELECT TOP 1 Id FROM [dbo].[Security_Contact] WHERE FirstName = 'System' AND LastName = 'Admin') as CreatedByContactId
           ,GETDATE() --<UpdatedDate, datetime,>
           ,NULL --<UpdatedByContactId, int,>
           ,LOWER(NEWID()) -- <ObjectId, varchar(40),>
END
GO


-- Add Permissions 
-- Add Permissions 
IF NOT EXISTS(SELECT 1 FROM  [dbo].[Security_Permission] WITH(NOLOCK)
          WHERE [Value] = 'Contact.ViewList')
BEGIN
INSERT INTO [dbo].[Security_Permission]
           ([Name]
           ,[Value]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     SELECT
           'Contact.ViewList' --<Name, varchar(100),>
           ,'Contact.ViewList' --<Value, varchar(100),>
           ,1 -- <Status, tinyint,>
           ,GETDATE() --<CreatedDate, datetime,>
           ,(SELECT TOP 1 Id FROM [dbo].[Security_Contact] WHERE FirstName = 'System' AND LastName = 'Admin') as CreatedByContactId
            ,GETDATE() --<UpdatedDate, datetime,>
           ,NULL --<UpdatedByContactId, int,>
           ,LOWER(NEWID()) -- <ObjectId, varchar(40),>		   
END
GO

IF NOT EXISTS(SELECT 1 FROM  [dbo].[Security_Permission] WITH(NOLOCK)
          WHERE [Value] = 'Contact.Add')
BEGIN
INSERT INTO [dbo].[Security_Permission]
           ([Name]
           ,[Value]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     SELECT
           'Contact.Add' --<Name, varchar(100),>
           ,'Contact.Add' --<Value, varchar(100),>
           ,1 -- <Status, tinyint,>
           ,GETDATE() --<CreatedDate, datetime,>
           ,(SELECT TOP 1 Id FROM [dbo].[Security_Contact] WHERE FirstName = 'System' AND LastName = 'Admin') as CreatedByContactId
            ,GETDATE() --<UpdatedDate, datetime,>
           ,NULL --<UpdatedByContactId, int,>
           ,LOWER(NEWID()) -- <ObjectId, varchar(40),>		   
END
GO

IF NOT EXISTS(SELECT 1 FROM  [dbo].[Security_Permission] WITH(NOLOCK)
          WHERE [Value] = 'Contact.Edit')
BEGIN
INSERT INTO [dbo].[Security_Permission]
           ([Name]
           ,[Value]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     SELECT
           'Contact.Edit' --<Name, varchar(100),>
           ,'Contact.Edit' --<Value, varchar(100),>
           ,1 -- <Status, tinyint,>
           ,GETDATE() --<CreatedDate, datetime,>
           ,(SELECT TOP 1 Id FROM [dbo].[Security_Contact] WHERE FirstName = 'System' AND LastName = 'Admin') as CreatedByContactId
            ,GETDATE() --<UpdatedDate, datetime,>
           ,NULL --<UpdatedByContactId, int,>
           ,LOWER(NEWID()) -- <ObjectId, varchar(40),>		   
END
GO


IF NOT EXISTS(SELECT 1 FROM  [dbo].[Security_Permission] WITH(NOLOCK)
          WHERE [Value] = 'Contact.Delete')
BEGIN
INSERT INTO [dbo].[Security_Permission]
           ([Name]
           ,[Value]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     SELECT
           'Contact.Delete' --<Name, varchar(100),>
           ,'Contact.Delete' --<Value, varchar(100),>
           ,1 -- <Status, tinyint,>
           ,GETDATE() --<CreatedDate, datetime,>
           ,(SELECT TOP 1 Id FROM [dbo].[Security_Contact] WHERE FirstName = 'System' AND LastName = 'Admin') as CreatedByContactId
            ,GETDATE() --<UpdatedDate, datetime,>
           ,NULL --<UpdatedByContactId, int,>
           ,LOWER(NEWID()) -- <ObjectId, varchar(40),>		   
END
GO

-- Add Menu Permission
IF NOT EXISTS(SELECT 1 FROM  [dbo].[Security_Permission] WITH(NOLOCK)
          WHERE [Value] = 'Menu.ViewList')
BEGIN
INSERT INTO [dbo].[Security_Permission]
           ([Name]
           ,[Value]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     SELECT
           'Menu.ViewList' --<Name, varchar(100),>
           ,'Menu.ViewList' --<Value, varchar(100),>
           ,1 -- <Status, tinyint,>
           ,GETDATE() --<CreatedDate, datetime,>
           ,(SELECT TOP 1 Id FROM [dbo].[Security_Contact] WHERE FirstName = 'System' AND LastName = 'Admin') as CreatedByContactId
            ,GETDATE() --<UpdatedDate, datetime,>
           ,NULL --<UpdatedByContactId, int,>
           ,LOWER(NEWID()) -- <ObjectId, varchar(40),>		   
END
GO

IF NOT EXISTS(SELECT 1 FROM  [dbo].[Security_Permission] WITH(NOLOCK)
          WHERE [Value] = 'Menu.Add')
BEGIN
INSERT INTO [dbo].[Security_Permission]
           ([Name]
           ,[Value]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     SELECT
           'Menu.Add' --<Name, varchar(100),>
           ,'Menu.Add' --<Value, varchar(100),>
           ,1 -- <Status, tinyint,>
           ,GETDATE() --<CreatedDate, datetime,>
           ,(SELECT TOP 1 Id FROM [dbo].[Security_Contact] WHERE FirstName = 'System' AND LastName = 'Admin') as CreatedByContactId
            ,GETDATE() --<UpdatedDate, datetime,>
           ,NULL --<UpdatedByContactId, int,>
           ,LOWER(NEWID()) -- <ObjectId, varchar(40),>		   
END
GO

IF NOT EXISTS(SELECT 1 FROM  [dbo].[Security_Permission] WITH(NOLOCK)
          WHERE [Value] = 'Menu.Edit')
BEGIN
INSERT INTO [dbo].[Security_Permission]
           ([Name]
           ,[Value]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     SELECT
           'Menu.Edit' --<Name, varchar(100),>
           ,'Menu.Edit' --<Value, varchar(100),>
           ,1 -- <Status, tinyint,>
           ,GETDATE() --<CreatedDate, datetime,>
           ,(SELECT TOP 1 Id FROM [dbo].[Security_Contact] WHERE FirstName = 'System' AND LastName = 'Admin') as CreatedByContactId
            ,GETDATE() --<UpdatedDate, datetime,>
           ,NULL --<UpdatedByContactId, int,>
           ,LOWER(NEWID()) -- <ObjectId, varchar(40),>		   
END
GO


IF NOT EXISTS(SELECT 1 FROM  [dbo].[Security_Permission] WITH(NOLOCK)
          WHERE [Value] = 'Menu.Delete')
BEGIN
INSERT INTO [dbo].[Security_Permission]
           ([Name]
           ,[Value]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     SELECT
           'Menu.Delete' --<Name, varchar(100),>
           ,'Menu.Delete' --<Value, varchar(100),>
           ,1 -- <Status, tinyint,>
           ,GETDATE() --<CreatedDate, datetime,>
           ,(SELECT TOP 1 Id FROM [dbo].[Security_Contact] WHERE FirstName = 'System' AND LastName = 'Admin') as CreatedByContactId
            ,GETDATE() --<UpdatedDate, datetime,>
           ,NULL --<UpdatedByContactId, int,>
           ,LOWER(NEWID()) -- <ObjectId, varchar(40),>		   
END
GO

-- Add Role Permission
INSERT INTO [dbo].[Security_RolePermission]
           ([RoleId]
           ,[PermissionId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     SELECT
            (SELECT TOP 1 Id FROM  [dbo].[Security_Role] WITH(NOLOCK) WHERE [Name] = 'System Admin')
           ,(SELECT TOP 1 Id FROM  [dbo].[Security_Permission] WITH(NOLOCK) WHERE [Value] = 'Menu.ViewList')
           ,GETDATE() -- <UpdatedDate, datetime,>
           ,(SELECT TOP 1 Id FROM [dbo].[Security_Contact] WHERE FirstName = 'System' AND LastName = 'Admin') as CreatedByContactId
           ,LOWER(NEWID())
GO

INSERT INTO [dbo].[Security_RolePermission]
           ([RoleId]
           ,[PermissionId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     SELECT
            (SELECT TOP 1 Id FROM  [dbo].[Security_Role] WITH(NOLOCK) WHERE [Name] = 'System Admin')
           ,(SELECT TOP 1 Id FROM  [dbo].[Security_Permission] WITH(NOLOCK) WHERE [Value] = 'Menu.Add')
           ,GETDATE() -- <UpdatedDate, datetime,>
           ,(SELECT TOP 1 Id FROM [dbo].[Security_Contact] WHERE FirstName = 'System' AND LastName = 'Admin') as CreatedByContactId
           ,LOWER(NEWID())
GO


INSERT INTO [dbo].[Security_RolePermission]
           ([RoleId]
           ,[PermissionId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     SELECT
            (SELECT TOP 1 Id FROM  [dbo].[Security_Role] WITH(NOLOCK) WHERE [Name] = 'System Admin')
           ,(SELECT TOP 1 Id FROM  [dbo].[Security_Permission] WITH(NOLOCK) WHERE [Value] = 'Menu.Edit')
           ,GETDATE() -- <UpdatedDate, datetime,>
           ,(SELECT TOP 1 Id FROM [dbo].[Security_Contact] WHERE FirstName = 'System' AND LastName = 'Admin') as CreatedByContactId
           ,LOWER(NEWID())
GO


INSERT INTO [dbo].[Security_RolePermission]
           ([RoleId]
           ,[PermissionId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     SELECT
            (SELECT TOP 1 Id FROM  [dbo].[Security_Role] WITH(NOLOCK) WHERE [Name] = 'System Admin')
           ,(SELECT TOP 1 Id FROM  [dbo].[Security_Permission] WITH(NOLOCK) WHERE [Value] = 'Menu.Delete')
           ,GETDATE() -- <UpdatedDate, datetime,>
           ,(SELECT TOP 1 Id FROM [dbo].[Security_Contact] WHERE FirstName = 'System' AND LastName = 'Admin') as CreatedByContactId
           ,LOWER(NEWID())
GO

-- Add User
IF NOT EXISTS(SELECT 1 FROM  [dbo].[Security_User] WITH(NOLOCK)
          WHERE [Username] = 'systemadmin')
BEGIN
INSERT INTO [dbo].[Security_User]
           ([CompanyId]
           ,[ContactId]
           ,[Username]
           ,[ProfileImageUrl]
           ,[PasswordHash]
           ,[PasswordSalt]
           ,[RegistrationToken]
           ,[PasswordResetToken]
           ,[LastLoginDateUtc]
           ,[LastLogoutDateUtc]
           ,[LastPasswordChangeUtc]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     SELECT
           (SELECT TOP 1 Id FROM [dbo].[Common_Company] WHERE Code = 'KTMSYS') as CompanyId   --<CompanyId, int,>
            ,(SELECT TOP 1 Id FROM [dbo].[Security_Contact] WHERE FirstName = 'System' AND LastName = 'Admin') as ContactId
           ,'systemadmin' --<Username, nvarchar(100),>
           ,NULL -- <ProfileImageUrl, nvarchar(250),>
           ,'iyXJQISMhJVUTQwmlD80R8KgF3E=' --<PasswordHash, nvarchar(100),>
           ,'RFUmxQ==' -- <PasswordSalt, nvarchar(100),>
           ,'' -- <RegistrationToken, nvarchar(50),>
           ,'' -- <PasswordResetToken, nvarchar(50),>
           ,NULL --<LastLoginDateUtc, datetime,>
           ,NULL --<LastLogoutDateUtc, datetime,>
           ,NULL --<LastPasswordChangeUtc, datetime,>
           ,1 -- <Status, int,>
           ,GETDATE() --<CreatedDate, datetime,>
            ,(SELECT TOP 1 Id FROM [dbo].[Security_Contact] WHERE FirstName = 'System' AND LastName = 'Admin') as CreatedByContactId
           ,NULL -- <UpdatedDate, datetime,>
           ,NULL --<UpdatedByContactId, int,>
           ,LOWER(NEWID())
END

-- Add User Roles
INSERT INTO [dbo].[Security_UserRole]
           ([UserId]
           ,[RoleId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
     SELECT
           (SELECT TOP 1 Id FROM [dbo].[Security_User] WITH(NOLOCK) WHERE [Username] = 'systemadmin') -- <UserId, int,>
           ,(SELECT TOP 1 Id FROM  [dbo].[Security_Role] WITH(NOLOCK) WHERE [Name] = 'System Admin')
           ,GETDATE() -- <UpdatedDate, datetime,>
           ,(SELECT TOP 1 Id FROM [dbo].[Security_Contact] WHERE FirstName = 'System' AND LastName = 'Admin') as UpdatedByContactId
          ,LOWER(NEWID())









