USE [master]
GO
/****** Object:  Database [KowToMateERP]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE DATABASE [KowToMateERP]
GO
USE [KowToMateERP]
GO
/****** Object:  UserDefinedFunction [dbo].[fnSplit]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object:  UserDefinedFunction [dbo].[fnSplit]    Script Date: 05/14/2014 16:11:10 ******/
-- =============================================
-- Author:		Aminul
-- Create date: 23 May 2022
-- Description: Split values from delimiter separated string like '1,2,3'.
-- Execution: SELECT * FROM [dbo].[Split]('1,2,3', ',')
-- =============================================
CREATE FUNCTION [dbo].[fnSplit](@list varchar(8000), @delimiter char(1))       
RETURNS 
@tableList TABLE(
	value NVARCHAR(100)
	)
AS
BEGIN
	DECLARE @value    NVARCHAR(100)
	DECLARE @position INT

	SET @list = LTRIM(RTRIM(@list)) + ','
	SET @position = CHARINDEX(@delimiter, @list, 1)

	IF REPLACE(@list, @delimiter, '') <> ''
	BEGIN
		WHILE @position > 0
		BEGIN
			SET @value = LTRIM(RTRIM(LEFT(@list, @position - 1)))
			IF @value <> ''
			BEGIN
				INSERT INTO @tableList (value) 
				VALUES (@value)
			END
			SET @list = RIGHT(@list, LEN(@list) - @position)
			SET @position = CHARINDEX(@delimiter, @list, 1)

		END
	END	
	RETURN
END



GO
/****** Object:  Table [dbo].[Common_ActivityLog]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Common_ActivityLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ActivityLogFor] [tinyint] NOT NULL,
	[PrimaryId] [int] NOT NULL,
	[Description] [varchar](500) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NOT NULL,
	[ObjectId] [varchar](504) NOT NULL,
 CONSTRAINT [PK_Common_ActivityLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Common_Company]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Common_Company](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [varchar](6) NOT NULL,
	[CompanyType] [tinyint] NOT NULL,
	[Telephone] [varchar](30) NULL,
	[Email] [varchar](30) NULL,
	[Address1] [varchar](100) NULL,
	[Address2] [varchar](100) NULL,
	[City] [varchar](30) NULL,
	[State] [varchar](30) NULL,
	[Zipcode] [varchar](10) NULL,
	[Country] [varchar](50) NULL,
	[Status] [smallint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Common_Country]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Common_Country](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Code] [varchar](6) NOT NULL,
	[ObjectId] [varchar](40) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Common_FileServer]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Common_FileServer](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[FileServerType] [tinyint] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[UserName] [varchar](100) NULL,
	[Password] [varchar](100) NULL,
	[AccessKey] [varchar](100) NULL,
	[SecretKey] [varchar](100) NULL,
	[RootFolder] [varchar](150) NULL,
	[SshKeyPath] [varchar](150) NULL,
	[Host] [varchar](150) NULL,
	[Protocol] [varchar](10) NULL,
	[Status] [tinyint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_Common_FileServer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Email_Queue]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Email_Queue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmailAccountId] [smallint] NULL,
	[FromEmail] [varchar](100) NULL,
	[FromEmailName] [varchar](100) NULL,
	[ToEmail] [varchar](100) NULL,
	[ToEmailName] [varchar](100) NULL,
	[CcEmail] [varchar](100) NULL,
	[BccEmail] [varchar](100) NULL,
	[Subject] [nvarchar](200) NULL,
	[Body] [nvarchar](max) NULL,
	[AttachmentFilePath] [varchar](250) NULL,
	[AttachedFileName] [varchar](100) NULL,
	[FileServerId] [smallint] NULL,
	[SentTries] [smallint] NULL,
	[SentOnDateTimeUtc] [datetime] NULL,
	[EmailPriority] [tinyint] NULL,
	[ThreadLock] [nvarchar](100) NULL,
	[DateProcessedUtc] [datetime] NULL,
	[Status] [tinyint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_Email_Queue] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Email_QueueArchive]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Email_QueueArchive](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmailAccountId] [smallint] NULL,
	[FromEmail] [varchar](100) NULL,
	[FromEmailName] [varchar](100) NULL,
	[ToEmail] [varchar](100) NULL,
	[ToEmailName] [varchar](100) NULL,
	[CcEmail] [varchar](100) NULL,
	[BccEmail] [varchar](100) NULL,
	[Subject] [nvarchar](200) NULL,
	[Body] [nvarchar](max) NULL,
	[AttachmentFilePath] [varchar](250) NULL,
	[AttachedFileName] [varchar](100) NULL,
	[FileServerId] [smallint] NULL,
	[SentTries] [smallint] NULL,
	[SentOnDateTimeUtc] [datetime] NULL,
	[EmailPriority] [tinyint] NULL,
	[ThreadLock] [nvarchar](100) NULL,
	[DateProcessedUtc] [datetime] NULL,
	[Status] [tinyint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_Email_QueueArchive] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Email_SenderAccount]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Email_SenderAccount](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Email] [varchar](100) NULL,
	[EmailDisplayName] [varchar](50) NULL,
	[MailServer] [varchar](50) NULL,
	[Port] [smallint] NULL,
	[UserName] [varchar](100) NULL,
	[Password] [varchar](100) NULL,
	[ApiKey] [varchar](100) NULL,
	[SecretKey] [varchar](100) NULL,
	[Domain] [varchar](100) NULL,
	[EnableSSL] [bit] NOT NULL,
	[UseDefaultCredentials] [bit] NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_Email_SenderAccount] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Email_Template]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Email_Template](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[SenderAccountId] [smallint] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[AccessCode] [varchar](100) NULL,
	[FromEmailAddress] [varchar](50) NULL,
	[BCCEmailAddresses] [varchar](50) NULL,
	[Subject] [varchar](200) NULL,
	[Body] [nvarchar](max) NULL,
	[Status] [smallint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_Email_Template] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HR_Designation]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HR_Designation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_Designation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Management_Team]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Management_Team](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_Management_Team] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Management_TeamMember]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Management_TeamMember](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[TeamId] [int] NULL,
	[TeamRoleId] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_Management_TeamMember] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Management_TeamRole]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Management_TeamRole](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NULL,
	[Name] [varchar](50) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactid] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedDateById] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_Management_TeamRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Security_CompanyPermission]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Security_CompanyPermission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyObjectId] [varchar](40) NOT NULL,
	[PermissionObjectId] [varchar](40) NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NULL,
 CONSTRAINT [PK_CompanyPermission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Security_CompanyTypePermission]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Security_CompanyTypePermission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyType] [tinyint] NOT NULL,
	[PermissionObjectId] [varchar](40) NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_CompanyTypePermission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Security_Contact]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Security_Contact](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NULL,
	[DesignationId] [int] NULL,
	[Email] [nvarchar](100) NULL,
	[Phone] [varchar](20) NULL,
	[ProfileImageUrl] [varchar](200) NULL,
	[Status] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Security_Menu]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Security_Menu](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[ParentId] [int] NULL,
	[Icon] [varchar](50) NULL,
	[IsLeftMenu] [bit] NOT NULL,
	[IsTopMenu] [bit] NOT NULL,
	[IsExternalMenu] [bit] NOT NULL,
	[MenuUrl] [varchar](150) NULL,
	[Status] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NULL,
	[DisplayOrder] [decimal](10, 2) NOT NULL,
 CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Security_MenuPermission]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Security_MenuPermission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MenuObjectId] [varchar](40) NULL,
	[PermissionObjectId] [varchar](40) NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedByContactId] [int] NOT NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_Security_MenuPermission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Security_Module]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Security_Module](
	[Id] [varchar](40) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Status] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
	[Icon] [varchar](50) NULL,
	[DisplayOrder] [decimal](10, 2) NOT NULL,
 CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Security_ModulePermission]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Security_ModulePermission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModuleObjectId] [varchar](40) NOT NULL,
	[PermissionObjectId] [varchar](40) NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_ModulePermission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Security_Permission]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Security_Permission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Value] [varchar](100) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_Permission_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Security_Role]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Security_Role](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
	[CompanyObjectId] [int] NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Security_RolePermission]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Security_RolePermission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleObjectId] [varchar](40) NOT NULL,
	[PermissionObjectId] [varchar](40) NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_RolePermission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Security_User]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Security_User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[ContactId] [int] NOT NULL,
	[Username] [nvarchar](100) NOT NULL,
	[ProfileImageUrl] [nvarchar](250) NULL,
	[PasswordHash] [nvarchar](100) NULL,
	[PasswordSalt] [nvarchar](100) NULL,
	[RegistrationToken] [nvarchar](50) NULL,
	[PasswordResetToken] [nvarchar](50) NULL,
	[LastLoginDateUtc] [datetime] NULL,
	[LastLogoutDateUtc] [datetime] NULL,
	[LastPasswordChangeUtc] [datetime] NULL,
	[Status] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Security_UserRole]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Security_UserRole](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[RoleId] [smallint] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedByContactId] [int] NOT NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SOP_StandardService]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SOP_StandardService](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[SortOrder] [bit] NULL,
	[Status] [tinyint] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedaDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_SOP_StandardService] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SOP_Template]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SOP_Template](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[FileServerId] [smallint] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Version] [smallint] NOT NULL,
	[ParentTemplateId] [int] NULL,
	[Instruction] [nvarchar](max) NULL,
	[UnitPrice] [decimal](10, 2) NULL,
	[Status] [tinyint] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[InstructionModifiedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_SOP_Template] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SOP_TemplateFile]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SOP_TemplateFile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SOPTemplateId] [int] NULL,
	[FileName] [varchar](100) NULL,
	[FileType] [varchar](20) NULL,
	[ActualPath] [varchar](300) NULL,
	[ModifiedPath] [varchar](300) NULL,
	[IsDeleted] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedByContactId] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[FileModifiedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NULL,
 CONSTRAINT [PK_SOP_TemplateFile] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SOP_TemplateService]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SOP_TemplateService](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SOPTemplateId] [int] NOT NULL,
	[SOPStandardServiceId] [smallint] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByContactId] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedByContactId] [int] NULL,
	[ObjectId] [varchar](40) NOT NULL,
 CONSTRAINT [PK_SOP_TemplateService] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Common_Company] ([Id], [Name], [Code], [CompanyType], [Telephone], [Email], [Address1], [Address2], [City], [State], [Zipcode], [Country], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (1, N'Kow  to Mate System', N'KTMSYS', 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, CAST(N'2022-05-20T18:00:00.423' AS DateTime), NULL, CAST(N'2022-05-23T17:28:06.463' AS DateTime), 1, N'412379d6-ccb3-46ff-a2ed-c86f15e321c0')
GO
INSERT [dbo].[Common_Company] ([Id], [Name], [Code], [CompanyType], [Telephone], [Email], [Address1], [Address2], [City], [State], [Zipcode], [Country], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (2, N'Kow  to Mate Base Client', N'KTMBCL', 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, CAST(N'2022-05-20T18:00:00.430' AS DateTime), NULL, CAST(N'2022-05-23T17:27:22.857' AS DateTime), 1, N'373f1da5-8b4d-4a97-b86e-744cccd50402')
GO
INSERT [dbo].[Common_Company] ([Id], [Name], [Code], [CompanyType], [Telephone], [Email], [Address1], [Address2], [City], [State], [Zipcode], [Country], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (3, N'Kow  to Mate Demo CLient', N'KTMDCL', 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, CAST(N'2022-05-20T18:00:00.440' AS DateTime), NULL, CAST(N'2022-05-23T17:27:47.130' AS DateTime), 1, N'd1b15ee1-5135-4042-b1cd-444e2f7632c4')
GO
SET IDENTITY_INSERT [dbo].[Email_SenderAccount] ON 
GO
INSERT [dbo].[Email_SenderAccount] ([Id], [Name], [Email], [EmailDisplayName], [MailServer], [Port], [UserName], [Password], [ApiKey], [SecretKey], [Domain], [EnableSSL], [UseDefaultCredentials], [IsDefault], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (1, N'Default Account', N'kowtomate@info.com', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, 1, CAST(N'2022-05-23T18:40:25.787' AS DateTime), 1, NULL, NULL, N'ec10fd02-e421-4a4c-84d1-7c8ac252811e')
GO
SET IDENTITY_INSERT [dbo].[Email_SenderAccount] OFF
GO
SET IDENTITY_INSERT [dbo].[Email_Template] ON 
GO
INSERT [dbo].[Email_Template] ([Id], [SenderAccountId], [Name], [AccessCode], [FromEmailAddress], [BCCEmailAddresses], [Subject], [Body], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (1, 1, N'Security.AccountVerification', N'Security.AccountVerification', N'info@kowtomate.com', NULL, NULL, NULL, 1, CAST(N'2022-05-23T18:41:48.113' AS DateTime), 1, CAST(N'2022-05-23T18:42:38.150' AS DateTime), 1, N'208e7c38-4a3f-485d-ae28-4f69ee7eba81')
GO
SET IDENTITY_INSERT [dbo].[Email_Template] OFF
GO
SET IDENTITY_INSERT [dbo].[Security_Contact] ON 
GO
INSERT [dbo].[Security_Contact] ([Id], [CompanyId], [FirstName], [LastName], [DesignationId], [Email], [Phone], [ProfileImageUrl], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (1, 1, N'System', N'Admin', NULL, NULL, NULL, NULL, 1, CAST(N'2022-05-20T18:00:00.443' AS DateTime), NULL, NULL, NULL, N'14d7ca70-dd9e-482d-a851-f668c9fe5ab3')
GO
SET IDENTITY_INSERT [dbo].[Security_Contact] OFF
GO
SET IDENTITY_INSERT [dbo].[Security_Menu] ON 
GO
INSERT [dbo].[Security_Menu] ([Id], [Name], [ParentId], [Icon], [IsLeftMenu], [IsTopMenu], [IsExternalMenu], [MenuUrl], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId], [DisplayOrder]) VALUES (1, N'Dashboard', NULL, N'bx bx-home-circle', 1, 0, 0, N'/dashboard', 1, CAST(N'2022-05-23T14:35:42.673' AS DateTime), 1, CAST(N'2022-05-23T18:26:09.053' AS DateTime), 1, N'ed296b93-ff0c-44da-bf97-48df6407e948', CAST(1.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Security_Menu] ([Id], [Name], [ParentId], [Icon], [IsLeftMenu], [IsTopMenu], [IsExternalMenu], [MenuUrl], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId], [DisplayOrder]) VALUES (2, N'Security', NULL, N'bx bx-lock', 1, 0, 0, NULL, 1, CAST(N'2022-05-23T14:37:57.347' AS DateTime), 1, CAST(N'2022-05-23T17:05:54.323' AS DateTime), 1, N'3381557d-8110-42fc-b34c-cbbd3b52edc8', CAST(2.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Security_Menu] ([Id], [Name], [ParentId], [Icon], [IsLeftMenu], [IsTopMenu], [IsExternalMenu], [MenuUrl], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId], [DisplayOrder]) VALUES (3, N'Menus', 2, NULL, 0, 0, 0, NULL, 1, CAST(N'2022-05-23T14:44:37.963' AS DateTime), 1, CAST(N'2022-05-23T17:06:38.133' AS DateTime), 1, N'8c2c56b7-a68e-48a6-801c-9284d3ec65a6', CAST(3.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Security_Menu] ([Id], [Name], [ParentId], [Icon], [IsLeftMenu], [IsTopMenu], [IsExternalMenu], [MenuUrl], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId], [DisplayOrder]) VALUES (4, N'Modules', 2, NULL, 1, 0, 0, NULL, 1, CAST(N'2022-05-23T14:47:07.243' AS DateTime), 1, CAST(N'2022-05-23T17:06:46.730' AS DateTime), 1, N'601484f6-74ea-484b-98c2-1331efd399de', CAST(4.00 AS Decimal(10, 2)))
GO
SET IDENTITY_INSERT [dbo].[Security_Menu] OFF
GO
SET IDENTITY_INSERT [dbo].[Security_MenuPermission] ON 
GO
INSERT [dbo].[Security_MenuPermission] ([Id], [MenuObjectId], [PermissionObjectId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (5, N'8c2c56b7-a68e-48a6-801c-9284d3ec65a6', N'ac1b8970-33da-4640-8cb9-3794392d4f7e', CAST(N'2022-05-23T17:06:38.140' AS DateTime), 1, N'4dfe4a57-392b-4569-aedf-8bfdca3a0031')
GO
INSERT [dbo].[Security_MenuPermission] ([Id], [MenuObjectId], [PermissionObjectId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (6, N'601484f6-74ea-484b-98c2-1331efd399de', N'bef7fb19-50d1-43b5-8ec0-aa12bd5452f5', CAST(N'2022-05-23T17:06:46.733' AS DateTime), 1, N'8bfe8d39-9873-4fbc-b529-7a9c674db5e2')
GO
INSERT [dbo].[Security_MenuPermission] ([Id], [MenuObjectId], [PermissionObjectId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (11, N'ed296b93-ff0c-44da-bf97-48df6407e948', N'ac78cf0b-c722-421a-ac8f-e3fa48c82c03', CAST(N'2022-05-23T18:26:09.053' AS DateTime), 1, N'3479b47b-39a2-4708-b914-491cd34b6f59')
GO
SET IDENTITY_INSERT [dbo].[Security_MenuPermission] OFF
GO
INSERT [dbo].[Security_Module] ([Id], [Name], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId], [Icon], [DisplayOrder]) VALUES (N'2', N'Security', 1, CAST(N'2022-05-23T13:56:20.717' AS DateTime), 1, NULL, NULL, N'2005f728-3f4e-4269-b1c0-d6c35388609f', N'fa fa-security', CAST(1.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Security_Module] ([Id], [Name], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId], [Icon], [DisplayOrder]) VALUES (N'3', N'Dashboard', 1, CAST(N'2022-05-23T14:57:43.367' AS DateTime), 1, NULL, NULL, N'ccae0253-4b91-42fe-ab03-b0ae4407694e', N'bx bx-home-circle', CAST(2.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Security_Module] ([Id], [Name], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId], [Icon], [DisplayOrder]) VALUES (N'4', N'Admin Settings', 1, CAST(N'2022-05-23T14:59:04.377' AS DateTime), 1, CAST(N'2022-05-23T14:59:49.217' AS DateTime), 1, N'70d16178-c069-486b-94de-00055c8f58ba', N'bx bx-cong', CAST(3.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Security_Module] ([Id], [Name], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId], [Icon], [DisplayOrder]) VALUES (N'5', N'HR', 1, CAST(N'2022-05-23T15:00:08.953' AS DateTime), 1, NULL, NULL, N'1075b23d-8901-4a75-b797-32590ee75ab3', NULL, CAST(4.00 AS Decimal(10, 2)))
GO
SET IDENTITY_INSERT [dbo].[Security_Permission] ON 
GO
INSERT [dbo].[Security_Permission] ([Id], [Name], [Value], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (1, N'Contact.ViewList', N'Contact.ViewList', 1, CAST(N'2022-05-20T18:03:25.800' AS DateTime), 1, CAST(N'2022-05-23T16:31:48.513' AS DateTime), 1, N'74b83c78-29f8-41b2-9eec-f323352cc324')
GO
INSERT [dbo].[Security_Permission] ([Id], [Name], [Value], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (2, N'Contact.Add', N'Contact.Add', 1, CAST(N'2022-05-20T18:04:39.550' AS DateTime), 1, CAST(N'2022-05-20T18:04:39.550' AS DateTime), NULL, N'cfd8c75d-8908-42bf-8e7e-7a84bc81d7d9')
GO
INSERT [dbo].[Security_Permission] ([Id], [Name], [Value], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (3, N'Contact.Edit', N'Contact.Edit', 1, CAST(N'2022-05-20T18:04:39.563' AS DateTime), 1, CAST(N'2022-05-20T18:04:39.563' AS DateTime), NULL, N'dc108a56-ccbd-4cad-8679-d3741d4e1192')
GO
INSERT [dbo].[Security_Permission] ([Id], [Name], [Value], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (4, N'Contact.Delete', N'Contact.Delete', 1, CAST(N'2022-05-20T18:04:39.573' AS DateTime), 1, CAST(N'2022-05-20T18:04:39.573' AS DateTime), NULL, N'393d77cd-8252-44d2-94ea-325a8d8db768')
GO
INSERT [dbo].[Security_Permission] ([Id], [Name], [Value], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (5, N'Security.ManageManus', N'Security.ManageManus', 1, CAST(N'2022-05-20T18:11:42.783' AS DateTime), 1, CAST(N'2022-05-23T13:12:39.300' AS DateTime), 1, N'ac1b8970-33da-4640-8cb9-3794392d4f7e')
GO
INSERT [dbo].[Security_Permission] ([Id], [Name], [Value], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (6, N'Security.ManagePermissions', N'Security.ManagePermissions', 1, CAST(N'2022-05-20T18:11:42.790' AS DateTime), 1, CAST(N'2022-05-23T13:12:05.807' AS DateTime), 1, N'1adcec12-6176-4684-b8d0-ab6d61c15a6b')
GO
INSERT [dbo].[Security_Permission] ([Id], [Name], [Value], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (7, N'Security.ManageModules', N'Security.ManageModules', 1, CAST(N'2022-05-20T18:11:42.837' AS DateTime), 1, CAST(N'2022-05-23T13:13:12.367' AS DateTime), 1, N'bef7fb19-50d1-43b5-8ec0-aa12bd5452f5')
GO
INSERT [dbo].[Security_Permission] ([Id], [Name], [Value], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (8, N'Common.ManageCountries', N'Common.ManageCountries', 1, CAST(N'2022-05-20T18:11:42.850' AS DateTime), 1, CAST(N'2022-05-23T13:14:57.720' AS DateTime), 1, N'b462af6a-a12e-4685-a8ae-6f193d9401cf')
GO
INSERT [dbo].[Security_Permission] ([Id], [Name], [Value], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (9, N'Common.ManageFileServers', N'Common.ManageFileServers', 1, CAST(N'2022-05-23T13:14:39.150' AS DateTime), 1, NULL, NULL, N'3fc8596a-29e0-476a-83f9-7242ae7be300')
GO
INSERT [dbo].[Security_Permission] ([Id], [Name], [Value], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (10, N'Common.ManageDesignations', N'Common.ManageDesignations', 1, CAST(N'2022-05-23T13:17:20.047' AS DateTime), 1, NULL, NULL, N'60e86eb9-92bc-406a-986e-9dd48e1cd814')
GO
INSERT [dbo].[Security_Permission] ([Id], [Name], [Value], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (11, N'Dashboard.ViewAdminDashboard', N'Dashboard.ViewAdminDashboard', 1, CAST(N'2022-05-23T16:10:57.123' AS DateTime), 1, NULL, NULL, N'ac78cf0b-c722-421a-ac8f-e3fa48c82c03')
GO
SET IDENTITY_INSERT [dbo].[Security_Permission] OFF
GO
SET IDENTITY_INSERT [dbo].[Security_Role] ON 
GO
INSERT [dbo].[Security_Role] ([Id], [Name], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId], [CompanyObjectId]) VALUES (1, N'System Admin', 1, CAST(N'2022-05-20T18:00:00.450' AS DateTime), 1, CAST(N'2022-05-20T18:00:00.450' AS DateTime), NULL, N'52160753-d1a2-49de-8e3c-a39fffcac6f4', 1)
GO
SET IDENTITY_INSERT [dbo].[Security_Role] OFF
GO
SET IDENTITY_INSERT [dbo].[Security_User] ON 
GO
INSERT [dbo].[Security_User] ([Id], [CompanyId], [ContactId], [Username], [ProfileImageUrl], [PasswordHash], [PasswordSalt], [RegistrationToken], [PasswordResetToken], [LastLoginDateUtc], [LastLogoutDateUtc], [LastPasswordChangeUtc], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (1, 1, 1, N'systemadmin', NULL, N'iyXJQISMhJVUTQwmlD80R8KgF3E=', N'RFUmxQ==', N'', N'', NULL, NULL, NULL, 1, CAST(N'2022-05-20T18:17:22.677' AS DateTime), 1, NULL, NULL, N'9b2fc036-ed01-4b06-9f2f-594a45246bda')
GO
SET IDENTITY_INSERT [dbo].[Security_User] OFF
GO
SET IDENTITY_INSERT [dbo].[Security_UserRole] ON 
GO
INSERT [dbo].[Security_UserRole] ([Id], [UserId], [RoleId], [UpdatedDate], [UpdatedByContactId], [ObjectId]) VALUES (1, 1, 1, CAST(N'2022-05-20T18:20:19.960' AS DateTime), 1, N'64197ffb-09e0-4feb-8bd0-da2431bac488')
GO
SET IDENTITY_INSERT [dbo].[Security_UserRole] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Common_Company_ObjectId]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Common_Company_ObjectId] ON [dbo].[Common_Company]
(
	[ObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Company_Code]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Company_Code] ON [dbo].[Common_Company]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UC_Country_Code]    Script Date: 5/23/2022 6:47:34 PM ******/
ALTER TABLE [dbo].[Common_Country] ADD  CONSTRAINT [UC_Country_Code] UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UC_Country_Name]    Script Date: 5/23/2022 6:47:34 PM ******/
ALTER TABLE [dbo].[Common_Country] ADD  CONSTRAINT [UC_Country_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Country]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Country] ON [dbo].[Common_Country]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Country_1]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Country_1] ON [dbo].[Common_Country]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Country_2]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE NONCLUSTERED INDEX [IX_Country_2] ON [dbo].[Common_Country]
(
	[ObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UC_FileServer_Name]    Script Date: 5/23/2022 6:47:34 PM ******/
ALTER TABLE [dbo].[Common_FileServer] ADD  CONSTRAINT [UC_FileServer_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Email_SenderAccount]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Email_SenderAccount] ON [dbo].[Email_SenderAccount]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UC_Template_AccessCode]    Script Date: 5/23/2022 6:47:34 PM ******/
ALTER TABLE [dbo].[Email_Template] ADD  CONSTRAINT [UC_Template_AccessCode] UNIQUE NONCLUSTERED 
(
	[AccessCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Email_Template]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Email_Template] ON [dbo].[Email_Template]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Designation]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Designation] ON [dbo].[HR_Designation]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Menu]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Menu] ON [dbo].[Security_Menu]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Security_Menu_ObjectId]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Security_Menu_ObjectId] ON [dbo].[Security_Menu]
(
	[ObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Module]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Module] ON [dbo].[Security_Module]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Module_1]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Module_1] ON [dbo].[Security_Module]
(
	[ObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ModulePermission]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_ModulePermission] ON [dbo].[Security_ModulePermission]
(
	[ObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Permission_Name]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Permission_Name] ON [dbo].[Security_Permission]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Permission_Value]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Permission_Value] ON [dbo].[Security_Permission]
(
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Security_Permission_ObjectId]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Security_Permission_ObjectId] ON [dbo].[Security_Permission]
(
	[ObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Role_CompanyIdAndRoleName]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Role_CompanyIdAndRoleName] ON [dbo].[Security_Role]
(
	[Name] ASC,
	[CompanyObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Security_Role_ObjectId]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Security_Role_ObjectId] ON [dbo].[Security_Role]
(
	[ObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_RolePermission]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_RolePermission] ON [dbo].[Security_RolePermission]
(
	[ObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SOP_StandardService]    Script Date: 5/23/2022 6:47:34 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_SOP_StandardService] ON [dbo].[SOP_StandardService]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Email_Queue]  WITH CHECK ADD  CONSTRAINT [FK_Email_Queue_Common_FileServer] FOREIGN KEY([FileServerId])
REFERENCES [dbo].[Common_FileServer] ([Id])
GO
ALTER TABLE [dbo].[Email_Queue] CHECK CONSTRAINT [FK_Email_Queue_Common_FileServer]
GO
ALTER TABLE [dbo].[Email_Queue]  WITH CHECK ADD  CONSTRAINT [FK_Email_Queue_Email_Queue] FOREIGN KEY([EmailAccountId])
REFERENCES [dbo].[Email_SenderAccount] ([Id])
GO
ALTER TABLE [dbo].[Email_Queue] CHECK CONSTRAINT [FK_Email_Queue_Email_Queue]
GO
ALTER TABLE [dbo].[Email_QueueArchive]  WITH CHECK ADD  CONSTRAINT [FK_Email_QueueArchive_Common_FileServer] FOREIGN KEY([FileServerId])
REFERENCES [dbo].[Common_FileServer] ([Id])
GO
ALTER TABLE [dbo].[Email_QueueArchive] CHECK CONSTRAINT [FK_Email_QueueArchive_Common_FileServer]
GO
ALTER TABLE [dbo].[Email_QueueArchive]  WITH CHECK ADD  CONSTRAINT [FK_Email_QueueArchive_Email_SenderAccount] FOREIGN KEY([EmailAccountId])
REFERENCES [dbo].[Email_SenderAccount] ([Id])
GO
ALTER TABLE [dbo].[Email_QueueArchive] CHECK CONSTRAINT [FK_Email_QueueArchive_Email_SenderAccount]
GO
ALTER TABLE [dbo].[Management_Team]  WITH CHECK ADD  CONSTRAINT [FK_Management_Team_Common_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Common_Company] ([Id])
GO
ALTER TABLE [dbo].[Management_Team] CHECK CONSTRAINT [FK_Management_Team_Common_Company]
GO
ALTER TABLE [dbo].[Management_TeamMember]  WITH CHECK ADD  CONSTRAINT [FK_Management_TeamMember_Management_Team] FOREIGN KEY([TeamId])
REFERENCES [dbo].[Management_Team] ([Id])
GO
ALTER TABLE [dbo].[Management_TeamMember] CHECK CONSTRAINT [FK_Management_TeamMember_Management_Team]
GO
ALTER TABLE [dbo].[Management_TeamMember]  WITH CHECK ADD  CONSTRAINT [FK_Management_TeamMember_Management_TeamRole] FOREIGN KEY([TeamRoleId])
REFERENCES [dbo].[Management_TeamRole] ([Id])
GO
ALTER TABLE [dbo].[Management_TeamMember] CHECK CONSTRAINT [FK_Management_TeamMember_Management_TeamRole]
GO
ALTER TABLE [dbo].[Management_TeamMember]  WITH CHECK ADD  CONSTRAINT [FK_Management_TeamMember_Security_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[Security_User] ([Id])
GO
ALTER TABLE [dbo].[Management_TeamMember] CHECK CONSTRAINT [FK_Management_TeamMember_Security_User]
GO
ALTER TABLE [dbo].[Management_TeamRole]  WITH CHECK ADD  CONSTRAINT [FK_Management_TeamRole_Common_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Common_Company] ([Id])
GO
ALTER TABLE [dbo].[Management_TeamRole] CHECK CONSTRAINT [FK_Management_TeamRole_Common_Company]
GO
ALTER TABLE [dbo].[Security_CompanyPermission]  WITH CHECK ADD  CONSTRAINT [FK_Security_CompanyPermission_Common_Company] FOREIGN KEY([CompanyObjectId])
REFERENCES [dbo].[Common_Company] ([ObjectId])
GO
ALTER TABLE [dbo].[Security_CompanyPermission] CHECK CONSTRAINT [FK_Security_CompanyPermission_Common_Company]
GO
ALTER TABLE [dbo].[Security_CompanyPermission]  WITH CHECK ADD  CONSTRAINT [FK_Security_CompanyPermission_Security_Permission] FOREIGN KEY([PermissionObjectId])
REFERENCES [dbo].[Security_Permission] ([ObjectId])
GO
ALTER TABLE [dbo].[Security_CompanyPermission] CHECK CONSTRAINT [FK_Security_CompanyPermission_Security_Permission]
GO
ALTER TABLE [dbo].[Security_CompanyTypePermission]  WITH CHECK ADD  CONSTRAINT [FK_CompanyTypePermission_Permission] FOREIGN KEY([PermissionObjectId])
REFERENCES [dbo].[Security_Permission] ([ObjectId])
GO
ALTER TABLE [dbo].[Security_CompanyTypePermission] CHECK CONSTRAINT [FK_CompanyTypePermission_Permission]
GO
ALTER TABLE [dbo].[Security_Contact]  WITH CHECK ADD  CONSTRAINT [FK_Contact_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Common_Company] ([Id])
GO
ALTER TABLE [dbo].[Security_Contact] CHECK CONSTRAINT [FK_Contact_Company]
GO
ALTER TABLE [dbo].[Security_Contact]  WITH CHECK ADD  CONSTRAINT [FK_Contact_Designation] FOREIGN KEY([DesignationId])
REFERENCES [dbo].[HR_Designation] ([Id])
GO
ALTER TABLE [dbo].[Security_Contact] CHECK CONSTRAINT [FK_Contact_Designation]
GO
ALTER TABLE [dbo].[Security_Menu]  WITH CHECK ADD  CONSTRAINT [FK_Menu_Menu] FOREIGN KEY([ParentId])
REFERENCES [dbo].[Security_Menu] ([Id])
GO
ALTER TABLE [dbo].[Security_Menu] CHECK CONSTRAINT [FK_Menu_Menu]
GO
ALTER TABLE [dbo].[Security_MenuPermission]  WITH CHECK ADD  CONSTRAINT [FK_Security_MenuPermission_Security_MenuPermission] FOREIGN KEY([MenuObjectId])
REFERENCES [dbo].[Security_Menu] ([ObjectId])
GO
ALTER TABLE [dbo].[Security_MenuPermission] CHECK CONSTRAINT [FK_Security_MenuPermission_Security_MenuPermission]
GO
ALTER TABLE [dbo].[Security_MenuPermission]  WITH CHECK ADD  CONSTRAINT [FK_Security_MenuPermission_Security_Permission] FOREIGN KEY([PermissionObjectId])
REFERENCES [dbo].[Security_Permission] ([ObjectId])
GO
ALTER TABLE [dbo].[Security_MenuPermission] CHECK CONSTRAINT [FK_Security_MenuPermission_Security_Permission]
GO
ALTER TABLE [dbo].[Security_ModulePermission]  WITH CHECK ADD  CONSTRAINT [FK_ModulePermission_Module] FOREIGN KEY([ModuleObjectId])
REFERENCES [dbo].[Security_Module] ([ObjectId])
GO
ALTER TABLE [dbo].[Security_ModulePermission] CHECK CONSTRAINT [FK_ModulePermission_Module]
GO
ALTER TABLE [dbo].[Security_ModulePermission]  WITH CHECK ADD  CONSTRAINT [FK_Security_ModulePermission_Security_Permission] FOREIGN KEY([PermissionObjectId])
REFERENCES [dbo].[Security_Permission] ([ObjectId])
GO
ALTER TABLE [dbo].[Security_ModulePermission] CHECK CONSTRAINT [FK_Security_ModulePermission_Security_Permission]
GO
ALTER TABLE [dbo].[Security_Role]  WITH CHECK ADD  CONSTRAINT [FK_Role_Company] FOREIGN KEY([CompanyObjectId])
REFERENCES [dbo].[Common_Company] ([Id])
GO
ALTER TABLE [dbo].[Security_Role] CHECK CONSTRAINT [FK_Role_Company]
GO
ALTER TABLE [dbo].[Security_RolePermission]  WITH CHECK ADD  CONSTRAINT [FK_Security_RolePermission_Security_Permission] FOREIGN KEY([PermissionObjectId])
REFERENCES [dbo].[Security_Permission] ([ObjectId])
GO
ALTER TABLE [dbo].[Security_RolePermission] CHECK CONSTRAINT [FK_Security_RolePermission_Security_Permission]
GO
ALTER TABLE [dbo].[Security_RolePermission]  WITH CHECK ADD  CONSTRAINT [FK_Security_RolePermission_Security_Role] FOREIGN KEY([RoleObjectId])
REFERENCES [dbo].[Security_Role] ([ObjectId])
GO
ALTER TABLE [dbo].[Security_RolePermission] CHECK CONSTRAINT [FK_Security_RolePermission_Security_Role]
GO
ALTER TABLE [dbo].[Security_User]  WITH CHECK ADD  CONSTRAINT [FK_User_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Common_Company] ([Id])
GO
ALTER TABLE [dbo].[Security_User] CHECK CONSTRAINT [FK_User_Company]
GO
ALTER TABLE [dbo].[Security_User]  WITH CHECK ADD  CONSTRAINT [FK_User_Contact] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Security_Contact] ([Id])
GO
ALTER TABLE [dbo].[Security_User] CHECK CONSTRAINT [FK_User_Contact]
GO
ALTER TABLE [dbo].[Security_UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Security_Role] ([Id])
GO
ALTER TABLE [dbo].[Security_UserRole] CHECK CONSTRAINT [FK_UserRole_Role]
GO
ALTER TABLE [dbo].[Security_UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[Security_User] ([Id])
GO
ALTER TABLE [dbo].[Security_UserRole] CHECK CONSTRAINT [FK_UserRole_User]
GO
ALTER TABLE [dbo].[SOP_Template]  WITH CHECK ADD  CONSTRAINT [FK_SOP_Template_Common_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Common_Company] ([Id])
GO
ALTER TABLE [dbo].[SOP_Template] CHECK CONSTRAINT [FK_SOP_Template_Common_Company]
GO
ALTER TABLE [dbo].[SOP_Template]  WITH CHECK ADD  CONSTRAINT [FK_SOP_Template_Common_FileServer] FOREIGN KEY([FileServerId])
REFERENCES [dbo].[Common_FileServer] ([Id])
GO
ALTER TABLE [dbo].[SOP_Template] CHECK CONSTRAINT [FK_SOP_Template_Common_FileServer]
GO
ALTER TABLE [dbo].[SOP_TemplateFile]  WITH CHECK ADD  CONSTRAINT [FK_SOP_TemplateFile_SOP_Template] FOREIGN KEY([SOPTemplateId])
REFERENCES [dbo].[SOP_Template] ([Id])
GO
ALTER TABLE [dbo].[SOP_TemplateFile] CHECK CONSTRAINT [FK_SOP_TemplateFile_SOP_Template]
GO
ALTER TABLE [dbo].[SOP_TemplateService]  WITH CHECK ADD  CONSTRAINT [FK_SOP_TemplateService_SOP_StandardService] FOREIGN KEY([SOPStandardServiceId])
REFERENCES [dbo].[SOP_StandardService] ([Id])
GO
ALTER TABLE [dbo].[SOP_TemplateService] CHECK CONSTRAINT [FK_SOP_TemplateService_SOP_StandardService]
GO
ALTER TABLE [dbo].[SOP_TemplateService]  WITH CHECK ADD  CONSTRAINT [FK_SOP_TemplateService_SOP_Template] FOREIGN KEY([SOPTemplateId])
REFERENCES [dbo].[SOP_Template] ([Id])
GO
ALTER TABLE [dbo].[SOP_TemplateService] CHECK CONSTRAINT [FK_SOP_TemplateService_SOP_Template]
GO
/****** Object:  StoredProcedure [dbo].[Helper_GenerateClassWithProperties]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:			Md Aminul
-- Create date:		Nov 05, 2019
-- Description:		Get Document Categories with Document Counts
-- Exec: Helper_GeenrateClassWithProperties 'TutorialGroup'
-- =============================================
CREATE PROCEDURE [dbo].[Helper_GenerateClassWithProperties]
 @DBTableName varchar(100)
AS
BEGIN
	declare @TableName sysname = @DBTableName
	declare @Result varchar(max) = 'public class ' + @TableName + '
	{'
	select @Result = @Result + '
		public ' + ColumnType + NullableSign + ' ' + ColumnName + ' { get; set; } '
	from
	(
		select 
			replace(col.name, ' ', '_') ColumnName,
			column_id ColumnId,
			case typ.name 
				when 'bigint' then 'long'
				when 'binary' then 'byte[]'
				when 'bit' then 'bool'
				when 'char' then 'string'
				when 'date' then 'DateTime'
				when 'datetime' then 'DateTime'
				when 'datetime2' then 'DateTime'
				when 'datetimeoffset' then 'DateTimeOffset'
				when 'decimal' then 'decimal'
				when 'float' then 'double'
				when 'image' then 'byte[]'
				when 'int' then 'int'
				when 'money' then 'decimal'
				when 'nchar' then 'string'
				when 'ntext' then 'string'
				when 'numeric' then 'decimal'
				when 'nvarchar' then 'string'
				when 'real' then 'float'
				when 'smalldatetime' then 'DateTime'
				when 'smallint' then 'short'
				when 'smallmoney' then 'decimal'
				when 'text' then 'string'
				when 'time' then 'TimeSpan'
				when 'timestamp' then 'long'
				when 'tinyint' then 'byte'
				when 'uniqueidentifier' then 'Guid'
				when 'varbinary' then 'byte[]'
				when 'varchar' then 'string'
				else 'UNKNOWN_' + typ.name
			end ColumnType,
			case 
				when col.is_nullable = 1 and typ.name in ('bigint', 'bit', 'date', 'datetime', 'datetime2', 'datetimeoffset', 'decimal', 'float', 'int', 'money', 'numeric', 'real', 'smalldatetime', 'smallint', 'smallmoney', 'time', 'tinyint', 'uniqueidentifier') 
				then '?' 
				else '' 
			end NullableSign
		from sys.columns col
			join sys.types typ on
				col.system_type_id = typ.system_type_id AND col.user_type_id = typ.user_type_id
		where object_id = object_id(@TableName)
	) t
	order by ColumnId

	set @Result = @Result  + '
	
	}'

	print @Result

END



GO
/****** Object:  StoredProcedure [dbo].[Security_sp_Module_Update]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_sp_Module_Update](
           @Id  int,
           @Name varchar(100),
           @Status tinyint,
           @UpdatedByContactId int
)
AS
BEGIN  
   UPDATE [dbo].[Security_Module]
   SET 
      [Name] = @Name, 
      [Status] = @Status,
      [UpdatedDate] = SYSDATETIME(),
      [UpdatedByContactId] = @UpdatedByContactId
   
		
     WHERE Id = @Id
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Company_Delete]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Common_Company_Delete](
            @ObjectId  varchar(40)
        
)
AS
BEGIN  
    DELETE FROM  [dbo].[Common_Company] WHERE ObjectId = @ObjectId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Company_GetAll]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_Common_Company_GetAll]
AS
BEGIN  
	SELECT 
	        Id,
			Name, 
			Code,
			CompanyType, 
			ISNULL(Telephone, '') Telephone, 
			ISNULL(Email, '') Email,
			Address1, 
			Address2, 
			City, 
			State, 
			Zipcode,
			Country,
			Status,
			CreatedDate,
			CreatedByContactId,
			UpdatedDate,
			UpdatedByContactId,
			ObjectId
    FROM [dbo].[Common_Company]
END
GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Company_GetById]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[SP_Common_Company_GetById]
(
   @CompanyId as int
)
AS
BEGIN
SELECT * FROM [dbo].[Common_Company] where Id = @CompanyId

END




GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Company_GetByObjectId]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SP_Common_Company_GetByObjectId]
@ObjectId varchar(40)
AS
BEGIN  

	SELECT *
	FROM [dbo].[Common_Company] WHERE [ObjectId] = @ObjectId

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Company_Insert]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Common_Company_Insert](
             
            @Name  nvarchar(100),
            @Code  nvarchar(6),
            @CompanyType tinyint,
            @Telephone varchar(30),
			@Email varchar(30),
            @Address1 varchar(100),
            @Address2 varchar(100),
            @City varchar(30),
            @State varchar(30),
            @Zipcode varchar(10),
            @Country varchar(50),
            @Status int,
            @CreatedByContactId int,
            @ObjectId varchar(40)
)
AS
BEGIN  
    INSERT INTO [dbo].[Common_Company]
           (       
		    Name,
            Code,
            CompanyType,
            Telephone,
			Email,
            Address1,
            Address2,
            City,
            State,
            Zipcode,
            Country,
            Status,
			CreatedDate,
            CreatedByContactId, 
            ObjectId
           
           )
     VALUES
          (
		    @Name,
            @Code,
            @CompanyType,
            @Telephone,
			@Email,
            @Address1,
            @Address2,
            @City,
            @State,
            @Zipcode,
            @Country,
            @Status,
			SYSDATETIME(),
            @CreatedByContactId, 
            @ObjectId
          
		   )

	SELECT SCOPE_IDENTITY();
END


GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Company_Update]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Common_Company_Update](
            @Id  int,
            @Name  nvarchar(100),
            @Code  nvarchar(6),
            @CompanyType tinyint,
            @Telephone varchar(30),
			@Email varchar(30),
            @Address1 varchar(100),
            @Address2 varchar(100),
            @City varchar(30),
            @State varchar(30),
            @Zipcode varchar(10),
            @Country varchar(50),
            @Status int,
            @UpdatedByContactId int
)
AS
BEGIN  
    UPDATE [dbo].[Common_Company]
    SET
	    Name = @Name, 
        Code = @Code,
		CompanyType= @CompanyType,
		Telephone = @Telephone,
		Email = @Email,
		Address1=@Address1,
		Address2= @Address2,
		City =@City,
		State = @State,
		Zipcode =@Zipcode ,
		Country = @Country,
		Status = @Status,
		UpdatedDate = SYSDATETIME(),
		UpdatedByContactId =@UpdatedByContactId
		
     WHERE Id = @Id
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Country_Delete]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Common_Country_Delete](
            @ObjectId  varchar(40)
        
)
AS
BEGIN  
    DELETE FROM  [dbo].[Common_Country] WHERE ObjectId = @ObjectId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Country_GetAll]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Common_Country_GetAll]


AS
BEGIN  
	SELECT  [Id]
      ,[Name]
      ,[Code]
      ,[ObjectId]
	  ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
  FROM [dbo].[Common_Country]
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Country_GetById]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[SP_Common_Country_GetById]
(
   @CountryId as int
)
AS
BEGIN
SELECT * FROM [dbo].[Common_Country] where Id = @CountryId

END




GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Country_GetByObjectId]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SP_Common_Country_GetByObjectId]
@ObjectId varchar(40)
AS
BEGIN  

	SELECT ID,
			Name,
			Code,
			ObjectId,
			CreatedDate,
			CreatedByContactId,
			UpdatedDate,
			UpdatedByContactId
	FROM [dbo].[Common_Country] WHERE [ObjectId] = @ObjectId

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Country_Insert]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Common_Country_Insert](
       @Name varchar(100),
       @Code varchar(6),
	   @ObjectId varchar(40),
       @CreatedByContactId int
)
AS
BEGIN  
	INSERT INTO [dbo].[Common_Country]
           ([Name]
           ,[Code]
           ,[ObjectId]
           ,[CreatedDate]
           ,[CreatedByContactId]
          )
     VALUES
           (
		   @Name,
           @Code,
           @ObjectId,
           SYSDATETIME(), 
           @CreatedByContactId
		   )


	 SELECT SCOPE_IDENTITY();
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Country_Update]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Common_Country_Update](
       @Id int,
       @Name varchar(100),
       @Code varchar(6),
       @UpdatedByContactId int
)
AS
BEGIN  
  UPDATE [dbo].[Common_Country]    
   SET 
	   [Name] = @Name,
       [Code] = @Code, 
       [UpdatedDate] = SYSDATETIME(),
       [UpdatedByContactId] = @UpdatedByContactId
       WHERE Id = @Id
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Common_FileServer_Delete]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SP_Common_FileServer_Delete](
            @ObjectId  varchar(40) 
)
AS
BEGIN  
    DELETE FROM [dbo].[Common_FileServer] WHERE ObjectId = @ObjectId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Common_FileServer_GetAll]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SP_Common_FileServer_GetAll]

AS
BEGIN  

	SELECT * FROM [dbo].[Common_FileServer]

END






GO
/****** Object:  StoredProcedure [dbo].[SP_Common_FileServer_GetById]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[SP_Common_FileServer_GetById]
(
   @FileServerId as int
)
AS
BEGIN

	SELECT * FROM [dbo].[Common_FileServer] where Id = @FileServerId

END




GO
/****** Object:  StoredProcedure [dbo].[SP_Common_FileServer_GetByObjectId]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SP_Common_FileServer_GetByObjectId]
@ObjectId varchar(40)
AS
BEGIN  

	SELECT Id, FileServerType, Name, UserName, Password, AccessKey, SecretKey,
				RootFolder, RootFolder, SshKeyPath, Host, Protocol,
				Status, CreatedDate, CreatedByContactId,
				UpdatedDate, UpdatedByContactId, ObjectId
	FROM [dbo].[Common_FileServer] WHERE [ObjectId] = @ObjectId

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Common_FileServer_Insert]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SP_Common_FileServer_Insert](
	  @FileServerType tinyint,
      @Name varchar(100),
      @UserName varchar(100),
      @Password varchar(100),
      @AccessKey varchar(100),
      @SecretKey varchar(100),
      @RootFolder varchar(150),
      @SshKeyPath varchar(150),
      @Host varchar(150),
      @Protocol varchar(10),
      @Status tinyint,
      @CreatedByContactId int,
      @ObjectId varchar(40)

)
AS
BEGIN  

    Insert Into  [dbo].[Common_FileServer] 
	(
		  FileServerType,
		  Name,
		  UserName,
		  Password,
		  AccessKey,
		  SecretKey,
		  RootFolder,
		  SshKeyPath,
		  Host,
		  Protocol,
		  Status,
		  CreatedDate,
		  CreatedByContactId,
		  ObjectId
	)

	Values
	(
		  @FileServerType,
		  @Name,
		  @UserName,
		  @Password,
		  @AccessKey,
		  @SecretKey,
		  @RootFolder,
		  @SshKeyPath,
		  @Host,
		  @Protocol,
		  @Status,
		  SYSDATETIME(),
		  @CreatedByContactId, 
		  @ObjectId
	)
  
	SELECT SCOPE_IDENTITY()

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Common_FileServer_Update]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SP_Common_FileServer_Update](
	  @Id int,
	  @FileServerType tinyint,
      @Name varchar(100),
      @UserName varchar(100),
      @Password varchar(100),
      @AccessKey varchar(100),
      @SecretKey varchar(100),
      @RootFolder varchar(150),
      @SshKeyPath varchar(150),
      @Host varchar(150),
      @Protocol varchar(10),
      @Status tinyint,
      @UpdatedByContactId int

)
AS
BEGIN  

    UPDATE [dbo].[Common_FileServer]    
    SET 
	   FileServerType = @FileServerType,
       Name = @Name,
       UserName = @UserName,
       Password = @Password,
       AccessKey = @AccessKey,
       SecretKey = @SecretKey,
       RootFolder = @RootFolder,
       SshKeyPath = @SshKeyPath,
       Host = @Host,
       Protocol = @Protocol,
       Status = @Status,
       UpdatedDate = SYSDATETIME(),
       UpdatedByContactId = @UpdatedByContactId
    WHERE Id = @Id
  
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Email_SenderAccount_Delete]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SP_Email_SenderAccount_Delete](
            @ObjectId  varchar(40) 
)
AS
BEGIN  
    DELETE FROM [dbo].[Email_SenderAccount] WHERE ObjectId = @ObjectId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Email_SenderAccount_GetAll]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Email_SenderAccount_GetAll]

AS
BEGIN  
	SELECT  [Id]
      ,[Name]
	  ,[Email]
	  ,[EmailDisplayName]
	  ,[MailServer]
	  ,[Port]
	  ,[UserName]
	  ,[Password]
	  ,[ApiKey]
	  ,[SecretKey]
	  ,[Domain]
	  ,[EnableSSL]
	  ,[UseDefaultCredentials]
	  ,[IsDefault]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Email_SenderAccount]
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Email_SenderAccount_GetById]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Email_SenderAccount_GetById]
@SenderAccountId int
AS
BEGIN  
	SELECT  [Id]
      ,[Name]
	  ,[Email]
	  ,[EmailDisplayName]
	  ,[MailServer]
	  ,[Port]
	  ,[UserName]
	  ,[Password]
	  ,[ApiKey]
	  ,[SecretKey]
	  ,[Domain]
	  ,[EnableSSL]
	  ,[UseDefaultCredentials]
	  ,[IsDefault]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Email_SenderAccount] WHERE Id = @SenderAccountId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Email_SenderAccount_GetByObjectId]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Email_SenderAccount_GetByObjectId]
@ObjectId varchar(40)
AS
BEGIN  
	SELECT  [Id]
      ,[Name]
	  ,[Email]
	  ,[EmailDisplayName]
	  ,[MailServer]
	  ,[Port]
	  ,[UserName]
	  ,[Password]
	  ,[ApiKey]
	  ,[SecretKey]
	  ,[Domain]
	  ,[EnableSSL]
	  ,[UseDefaultCredentials]
	  ,[IsDefault]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Email_SenderAccount] WHERE ObjectId = @ObjectId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Email_SenderAccount_Insert]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SP_Email_SenderAccount_Insert]
	  @Name varchar(100),
	  @Email varchar(100),
	  @EmailDisplayName varchar(50),
	  @MailServer varchar(50),
	  @Port smallint,
	  @UserName varchar(100),
	  @Password varchar(100),
	  @ApiKey varchar(100),
	  @SecretKey varchar(100),
	  @Domain varchar(100),
	  @EnableSSL bit,
	  @UseDefaultCredentials bit,
	  @IsDefault bit,
      @Status tinyint,
      @CreatedByContactId int,
      @ObjectId varchar(40)
AS
BEGIN  

	Insert Into  [dbo].[Email_SenderAccount] 
	(
		  [Name],
		  [Email],
		  [EmailDisplayName],
		  [MailServer],
		  [Port],
		  [UserName],
		  [Password],
		  [ApiKey],
		  [SecretKey],
		  [Domain],
		  [EnableSSL],
		  [UseDefaultCredentials],
		  [IsDefault],
		  [Status],
		  [CreatedDate],
		  [CreatedByContactId],
		  [ObjectId]
	)

	Values
	(
			@Name,
			@Email,
			@EmailDisplayName,
			@MailServer,
			@Port,
			@UserName,
			@Password,
			@ApiKey,
			@SecretKey,
			@Domain,
			@EnableSSL,
			@UseDefaultCredentials,
			@IsDefault,
			@Status,
			SYSDATETIME(),
			@CreatedByContactId,
			@ObjectId
	)

	SELECT SCOPE_IDENTITY();

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Email_SenderAccount_Update]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SP_Email_SenderAccount_Update]
	  @Id int,
	  @Name varchar(100),
	  @Email varchar(100),
	  @EmailDisplayName varchar(50),
	  @MailServer varchar(50),
	  @Port smallint,
	  @UserName varchar(100),
	  @Password varchar(100),
	  @ApiKey varchar(100),
	  @SecretKey varchar(100),
	  @Domain varchar(100),
	  @EnableSSL bit,
	  @UseDefaultCredentials bit,
	  @IsDefault bit,
      @Status tinyint,
      @UpdatedByContactId int
AS
BEGIN  

	Update [dbo].[Email_SenderAccount] 
	Set
		  [Name] = @Name,
		  [Email] = @Email,
		  [EmailDisplayName] = @EmailDisplayName,
		  [MailServer] = @MailServer,
		  [Port] = @Port,
		  [UserName] = @UserName,
		  [Password] = @Password,
		  [ApiKey] = @ApiKey,
		  [SecretKey] = @SecretKey,
		  [Domain] = @Domain,
		  [EnableSSL] = @EnableSSL,
		  [UseDefaultCredentials] = @UseDefaultCredentials,
		  [IsDefault] = @IsDefault,
		  [Status] = @Status,
		  [UpdatedByContactId] = @UpdatedByContactId,
		  [UpdatedDate] = SYSDATETIME()
	WHERE Id = @Id


END



GO
/****** Object:  StoredProcedure [dbo].[SP_Email_Template_Delete]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SP_Email_Template_Delete](
            @ObjectId  varchar(40) 
)
AS
BEGIN  
    DELETE FROM [dbo].[Email_Template] WHERE ObjectId = @ObjectId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Email_Template_GetAll]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Email_Template_GetAll]

AS
BEGIN  
	SELECT  [Id]
      ,[SenderAccountId]
	  ,[Name]
	  ,[AccessCode]
	  ,[FromEmailAddress]
	  ,[BCCEmailAddresses]
	  ,[Subject]
	  ,[Body]
	  ,[Status]
	  ,[CreatedDate]
	  ,[CreatedByContactId]
	  ,[UpdatedDate]
	  ,[UpdatedByContactId]
	  ,[ObjectId]
	FROM [dbo].[Email_Template]
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Email_Template_GetById]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SP_Email_Template_GetById]
@TemplateId int
AS
BEGIN  
	SELECT  [Id]
      ,[SenderAccountId]
	  ,[Name]
	  ,[AccessCode]
	  ,[FromEmailAddress]
	  ,[BCCEmailAddresses]
	  ,[Subject]
	  ,[Body]
	  ,[Status]
	  ,[CreatedDate]
	  ,[CreatedByContactId]
	  ,[UpdatedDate]
	  ,[UpdatedByContactId]
	  ,[ObjectId]
  FROM [dbo].[Email_Template] WHERE Id = @TemplateId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Email_Template_GetByObjectId]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Email_Template_GetByObjectId]
@ObjectId varchar(40)
AS
BEGIN  
	SELECT  [Id]
      ,[SenderAccountId]
	  ,[Name]
	  ,[AccessCode]
	  ,[FromEmailAddress]
	  ,[BCCEmailAddresses]
	  ,[Subject]
	  ,[Body]
	  ,[Status]
	  ,[CreatedDate]
	  ,[CreatedByContactId]
	  ,[UpdatedDate]
	  ,[UpdatedByContactId]
	  ,[ObjectId]
  FROM [dbo].[Email_Template] WHERE ObjectId = @ObjectId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Email_Template_Insert]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SP_Email_Template_Insert]
	  @SenderAccountId smallint,
	  @Name varchar(100),
	  @AccessCode varchar(100),
	  @FromEmailAddress varchar(50),
	  @BCCEmailAddresses varchar(50),
	  @Subject varchar(200),
	  @Body nvarchar(max),
      @Status smallint,
      @CreatedByContactId int,
      @ObjectId varchar(40)
AS
BEGIN  

	Insert Into  [dbo].[Email_Template] 
	(
		  [SenderAccountId],
		  [Name],
		  [AccessCode],
		  [FromEmailAddress],
		  [BCCEmailAddresses],
		  [Subject],
		  [Body],
		  [Status],
		  [CreatedDate],
		  [CreatedByContactId],
		  [ObjectId]
	)

	Values
	(
		  @SenderAccountId,
		  @Name,
		  @AccessCode,
		  @FromEmailAddress,
		  @BCCEmailAddresses,
		  @Subject,
		  @Body,
		  @Status,
		  SYSDATETIME(),
		  @CreatedByContactId,
		  @ObjectId
	)

	SELECT SCOPE_IDENTITY();

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Email_Template_Update]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SP_Email_Template_Update]
	  @Id smallint,
	  @SenderAccountId smallint,
	  @Name varchar(100),
	  @AccessCode varchar(100),
	  @FromEmailAddress varchar(50),
	  @BCCEmailAddresses varchar(50),
	  @Subject varchar(200),
	  @Body nvarchar(max),
      @Status smallint,
	  @UpdatedByContactId int
AS
BEGIN  

	Update [dbo].[Email_Template] 
	Set
		  [SenderAccountId] = @SenderAccountId,
		  [Name] = @Name,
		  [AccessCode] = @AccessCode,
		  [FromEmailAddress] = @FromEmailAddress,
		  [BCCEmailAddresses] = @BCCEmailAddresses,
		  [Subject] = @Subject,
		  [Body] = @Body,
		  [Status] = @Status,
		  [UpdatedByContactId] = @UpdatedByContactId,
		  [UpdatedDate] = SYSDATETIME()
	WHERE Id = @Id


END



GO
/****** Object:  StoredProcedure [dbo].[SP_HR_Designation_Delete]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_HR_Designation_Delete](
            @ObjectId varchar(40)
        
)
AS
BEGIN  
    DELETE FROM  [dbo].[HR_Designation] WHERE ObjectId = @ObjectId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_HR_Designation_GetAll]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_HR_Designation_GetAll]

AS
BEGIN  
	SELECT  [Id]
      ,[Name]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[HR_Designation]
END



GO
/****** Object:  StoredProcedure [dbo].[SP_HR_Designation_GetById]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_HR_Designation_GetById]
@DesignationId int
AS
BEGIN  
	SELECT  [Id]
      ,[Name]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[HR_Designation] WHERE Id = @DesignationId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_HR_Designation_GetByObjectId]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SP_HR_Designation_GetByObjectId]
@ObjectId varchar(40)
AS
BEGIN  

	SELECT  [Id]
      ,[Name]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
	FROM [dbo].[HR_Designation] WHERE [ObjectId] = @ObjectId

END



GO
/****** Object:  StoredProcedure [dbo].[SP_HR_Designation_Insert]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_HR_Designation_Insert](
            @Name  nvarchar(100),
            @Status int,
			@ObjectId varchar(40),
            @CreatedByContactId int
)
AS
BEGIN  

    Insert Into  [dbo].[HR_Designation] 
	(
		Name,
		Status,
		CreatedDate,
		CreatedByContactId,
		ObjectId
	)

	Values
	(
	    @Name,
		@Status,
		SYSDATETIME(),
		@CreatedByContactId, 
		@ObjectId
	)

	SELECT SCOPE_IDENTITY();
  
END



GO
/****** Object:  StoredProcedure [dbo].[SP_HR_Designation_Update]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_HR_Designation_Update](
            @Id  int,
            @Name  nvarchar(100),
            @Status int,
            @UpdatedByContactId int
)
AS
BEGIN  
    UPDATE [dbo].[HR_Designation]
      SET 
	  [Name] = @Name,
      [Status] = @Status,    
      [UpdatedDate] = SYSDATETIME(),
      [UpdatedByContactId] = @UpdatedByContactId

      WHERE Id = @Id
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Management_Team_Delete]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SP_Management_Team_Delete](
            @ObjectId  varchar(40) 
)
AS
BEGIN  
    DELETE FROM [dbo].[Management_Team] WHERE ObjectId = @ObjectId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Management_Team_GetAll]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Management_Team_GetAll]

AS
BEGIN  
	SELECT  [Id]
      ,[CompanyId]
	  ,[Name]
	  ,[CreatedDate]
	  ,[CreatedByContactId]
	  ,[UpdatedDate]
	  ,[UpdatedByContactId]
	  ,[ObjectId]
	FROM [dbo].[Management_Team]
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Management_Team_GetById]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SP_Management_Team_GetById]
@TeamId int
AS
BEGIN  
	SELECT  [Id]
      ,[CompanyId]
	  ,[Name]
	  ,[CreatedDate]
	  ,[CreatedByContactId]
	  ,[UpdatedDate]
	  ,[UpdatedByContactId]
	  ,[ObjectId]
  FROM [dbo].[Management_Team] WHERE Id = @TeamId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Management_Team_GetByObjectId]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Management_Team_GetByObjectId]
@ObjectId varchar(40)
AS
BEGIN  
	SELECT  [Id]
      ,[CompanyId]
	  ,[Name]
	  ,[CreatedDate]
	  ,[CreatedByContactId]
	  ,[UpdatedDate]
	  ,[UpdatedByContactId]
	  ,[ObjectId]
  FROM [dbo].[Management_Team] WHERE ObjectId = @ObjectId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Management_Team_Insert]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SP_Management_Team_Insert]
	  @CompanyId int,
	  @Name varchar(50),
	  @CreatedByContactId int,
	  @ObjectId varchar(40)
AS
BEGIN  

	Insert Into  [dbo].[Management_Team] 
	(
		  [CompanyId],
		  [Name],
		  [CreatedDate],
		  [CreatedByContactId],
		  [ObjectId]
	)

	Values
	(
		  @CompanyId,
		  @Name,
		  SYSDATETIME(),
		  @CreatedByContactId,
		  @ObjectId
	)

	SELECT SCOPE_IDENTITY();

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Management_Team_Update]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SP_Management_Team_Update]
	  @Id int,
	  @CompanyId int,
	  @Name varchar(50),
	  @UpdatedByContactId int
AS
BEGIN  

	Update [dbo].[Management_Team] 
	Set
		  [CompanyId] = @CompanyId,
		  [Name] = @Name,
		  [UpdatedDate] = SYSDATETIME(),
		  [UpdatedByContactId] = @UpdatedByContactId
	WHERE Id = @Id


END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_Delete]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================
CREATE PROCEDURE [dbo].[SP_Security_Contact_Delete](
@ObjectId  varchar(40)        
)
AS
BEGIN  
    DELETE FROM  [dbo].[Security_Contact] WHERE ObjectId = @ObjectId
END

GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetAll]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================
CREATE PROCEDURE [dbo].[SP_Security_Contact_GetAll]
AS
BEGIN  
	SELECT [Id]
      ,[CompanyId]
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
      ,[ObjectId]
  FROM [dbo].[Security_Contact]
END

GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetById]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Contact_GetById]
@ContactId int
AS
BEGIN  
	SELECT [Id]
      ,[CompanyId]
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
      ,[ObjectId]
  FROM [dbo].[Security_Contact] Where Id= @ContactId
END

GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetByObjectId]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Md Aminul Islam
-- Create date: 16 May 2022
-- Description: Get Contact Details
-- =============================================
CREATE PROCEDURE [dbo].[SP_Security_Contact_GetByObjectId]
@ObjectId varchar(40)
AS
BEGIN  
	SELECT [Id]
      ,[CompanyId]
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
      ,[ObjectId]
  FROM [dbo].[Security_Contact] Where ObjectId= @ObjectId
END

GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetListWithDetails]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Md Aminul Islam
-- Create date: 16 May 2022
-- Description:	Get company name
-- =============================================
CREATE PROCEDURE [dbo].[SP_Security_Contact_GetListWithDetails]
AS
BEGIN  
	SELECT c.[Id]     
	   ,com.[Name] CompanyName
      ,c.[FirstName]
      ,c.[LastName]
	  ,d.[Name] DesignationName
      ,c.[Email]
      ,c.[Phone]
      ,c.[Status]
      ,c.[CreatedDate]
      ,c.[CreatedByContactId]
      ,c.[ObjectId]
  FROM [dbo].[Security_Contact] c WITH(NOLOCK)
  INNER JOIN [dbo].Common_Company com on com.Id = c.CompanyId
  LEFT JOIN [dbo].[HR_Designation] d on d.Id = c.DesignationId  
END

GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_Insert]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Contact_Insert](
             
			@CompanyId int,
			@FirstName nvarchar(100),
			@LastName nvarchar(100),
			@DesignationId int,
            @Email nvarchar(100),
            @Phone varchar(20),
            @ProfileImageUrl varchar(200),
            @Status int,
            @CreatedByContactId int,
            @ObjectId varchar(40)
)
AS
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
           ,[ObjectId])
     VALUES
           (
		   @CompanyId,
           @FirstName,
           @LastName, 
           @DesignationId, 
           @Email, 
           @Phone, 
           @ProfileImageUrl, 
           @Status, 
           SYSDATETIME(), 
           @CreatedByContactId,
           @ObjectId

		   )
END


GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_Update]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================
CREATE PROCEDURE [dbo].[SP_Security_Contact_Update](
    @ObjectId nvarchar(40),
	@FirstName nvarchar(100),
	@LastName nvarchar(100),
	@DesignationId int,
    @Email nvarchar(100),
    @Phone varchar(20),
    @Status int,
    @UpdatedByContactId int           
)
AS
BEGIN  
	UPDATE [dbo].[Security_Contact]
	SET 
		[FirstName] = @FirstName
		,[LastName] = @LastName
		,[DesignationId] = @DesignationId
		,[Email] = @Email
		,[Phone] = @Phone
		,[Status] = @Status
		,[UpdatedDate] = SYSDATETIME()
		,[UpdatedByContactId] = @UpdatedByContactId      
    WHERE ObjectId = @ObjectId
END


GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Menu_Delete]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_Security_Menu_Delete](
@ObjectId  varchar(40)       
)
AS
BEGIN  
    DELETE FROM  [dbo].[Security_Menu] WHERE ObjectId = @ObjectId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Menu_GetAll]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Menu_GetAll]

AS
BEGIN  
SELECT [Id]
      ,[Name]
      ,[ParentId]
      ,[Icon]
      ,[IsLeftMenu]
      ,[IsTopMenu]
      ,[IsExternalMenu]
      ,[MenuUrl]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_Menu]

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Menu_GetById]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Menu_GetById]
@MenuId int
AS
BEGIN  
SELECT [Id]
      ,[Name]
      ,[ParentId]
      ,[Icon]
      ,[IsLeftMenu]
      ,[IsTopMenu]
      ,[IsExternalMenu]
      ,[MenuUrl]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_Menu] WHERE Id = @MenuId

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Menu_GetByObjectId]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SP_Security_Menu_GetByObjectId]
@ObjectId varchar(40)
AS
BEGIN  
SELECT [Id]
      ,[Name]
      ,[ParentId]
      ,[Icon]
      ,[IsLeftMenu]
      ,[IsTopMenu]
      ,[IsExternalMenu]
      ,[MenuUrl]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_Menu] WHERE [ObjectId] = @ObjectId

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Menu_GetListWithDetails]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================
CREATE PROCEDURE [dbo].[SP_Security_Menu_GetListWithDetails]
AS
BEGIN  
SELECT m.[Id]
      ,m.[Name]
	  ,p.[Name] ParentMenuName
      ,m.[Icon]     
      ,m.[MenuUrl]
      ,m.[Status]
      ,m.[CreatedDate]     
      ,m.[ObjectId]
  FROM [dbo].[Security_Menu] m WITH(NOLOCK)
  LEFT JOIN [dbo].[Security_Menu] p WITH(NOLOCK) on p.Id = m.ParentId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Menu_Insert]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Menu_Insert](
             
           @Name varchar(100),
           @ParentId int,
           @Icon varchar(50),
           @IsLeftMenu bit,
           @IsTopMenu bit,
           @IsExternalMenu bit,
           @MenuUrl varchar(150),
           @Status int,
           @CreatedByContactId int,
           @ObjectId varchar(40)
)
AS
BEGIN  
   INSERT INTO [dbo].[Security_Menu]
           ([Name]
           ,[ParentId]
           ,[Icon]
           ,[IsLeftMenu]
           ,[IsTopMenu]
           ,[IsExternalMenu]
           ,[MenuUrl]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[ObjectId]
		   ,[DisplayOrder]
		   )
     VALUES
           (
		   @Name,
           @ParentId, 
           @Icon, 
           @IsLeftMenu, 
           @IsTopMenu,
           @IsExternalMenu,
           @MenuUrl, 
           @Status, 
           SYSDATETIME(),  
           @CreatedByContactId, 
           @ObjectId
		   ,ISNULL((SELECT MAX(DisplayOrder) FROM [dbo].[Security_Menu]),0) + 1
		   )

	SELECT SCOPE_IDENTITY()

END




GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Menu_Update]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Menu_Update](
           @Id  int,
           @Name varchar(100),
           @ParentId int,
           @Icon varchar(50),
           @IsLeftMenu bit,
           @IsTopMenu bit,
           @IsExternalMenu bit,
           @MenuUrl varchar(150),
           @Status int,
           @UpdatedByContactId int
)
AS
BEGIN  
   UPDATE [dbo].[Security_Menu]
   SET 
      [Name] = @Name, 
      [ParentId] = @ParentId,
      [Icon] = @Icon, 
      [IsLeftMenu] = @IsLeftMenu,
      [IsTopMenu] = @IsTopMenu, 
      [IsExternalMenu] = @IsExternalMenu,
      [MenuUrl] = @MenuUrl, 
      [Status] = @Status,
      [UpdatedDate] = SYSDATETIME(),
      [UpdatedByContactId] = @UpdatedByContactId
   
		
     WHERE Id = @Id
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_MenuPermission_InsertOrUpdateByMenuObjectId]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Md Aminul islam
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Exec: [dbo].[SP_Security_MenuPermission_InsertOrUpdateByMenuId] 1,'1,2,3',1
-- =============================================
CREATE PROCEDURE [dbo].[SP_Security_MenuPermission_InsertOrUpdateByMenuObjectId]
@MenuObjectId varchar(40),
@PermissionObjectIds varchar(5000),
@UpdatedByContactId INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE FROM [dbo].[Security_MenuPermission] WHERE MenuObjectId = @MenuObjectId

	INSERT INTO [dbo].[Security_MenuPermission]
           ([MenuObjectId]
           ,[PermissionObjectId]
           ,[UpdatedDate]
           ,[UpdatedByContactId]
           ,[ObjectId])
           SELECT @MenuObjectId,[value],GETDATE(),@UpdatedByContactId,
		   LOWER(NEWID()) FROM fnSplit(@PermissionObjectIds,',')
END
GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Module_Delete]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Module_Delete](
            @ModuleId  int
        
)
AS
BEGIN  
    DELETE FROM  [dbo].[Security_Module] WHERE Id = @ModuleId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Module_GetAll]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Module_GetAll]

AS
BEGIN  
SELECT [Id]
      ,[Name]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_Module]

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Module_GetById]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Module_GetById]
@ModuleId int
AS
BEGIN  
SELECT [Id]
      ,[Name]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_Module] Where Id = @ModuleId

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Module_Insert]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Md Aminul Islam
-- Create date: 23 May 2022
-- Description: Add New Module 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Module_Insert]( 
           @Name varchar(100),
		   @Icon varchar(50),
           @Status tinyint,
           @CreatedByContactId int,
           @ObjectId varchar(40)
)
AS
BEGIN  

DECLARE @DisplayOrder DECIMAL(10,2) = 1.0

SET @DisplayOrder =(SELECT ISNULL(MAX(DisplayOrder),0) FROM [dbo].[Security_Module]) 

INSERT INTO [dbo].[Security_Module]
           ([Name]
		   ,[Icon]
		   ,[DisplayOrder]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[ObjectId])
     VALUES
           (
		   @Name,
		   @Icon,
		   @DisplayOrder + 1,
           @Status, 
           SYSDATETIME(), 
           @CreatedByContactId, 
           @ObjectId
		   )
END

GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Module_Update]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Module_Update]( 
		   @Id int,
           @Name varchar(100),
           @Status int,
		   @UpdatedByContactId int
)
AS
BEGIN  

	UPDATE [dbo].[Security_Module]
    SET 
		[Name] = @Name,
		[Status] = @Status,
		[UpdatedDate] = SYSDATETIME(), 
		[UpdatedByContactId] = @UpdatedByContactId
     WHERE Id = @Id


END


GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Permission_Delete]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Permission_Delete](
            @ObjectId varchar(40)
        
)
AS
BEGIN  
    DELETE FROM  [dbo].[Security_Permission] WHERE ObjectId = @ObjectId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Permission_GetAll]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Permission_GetAll]

AS
BEGIN  
SELECT [Id]
      ,[Name]
	  ,[Value]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_Permission]

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Permission_GetAllByUserId]    Script Date: 5/23/2022 6:47:34 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_Security_Permission_GetById]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Permission_GetById]
@PermissionId int
AS
BEGIN  
SELECT [Id]
      ,[Name]
	  ,[Value]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_Permission] Where Id = @PermissionId

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Permission_GetByObjectId]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Permission_GetByObjectId]
@ObjectId varchar(40)
AS
BEGIN  
SELECT [Id]
      ,[Name]
	  ,[Value]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_Permission] Where ObjectId = @ObjectId

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Permission_Insert]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Permission_Insert]( 
           @Name varchar(100),
		   @Value varchar(100),
           @Status tinyint,
           @CreatedByContactId int,
           @ObjectId varchar(40)
)
AS
BEGIN  
INSERT INTO [dbo].[Security_Permission]
           ([Name]
		   ,[Value]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[ObjectId])
     VALUES
           (
		   @Name,
		   @Value,
           @Status, 
           SYSDATETIME(), 
           @CreatedByContactId, 
           @ObjectId
		   )
END


GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Permission_Update]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Permission_Update](
           @Id  int,
           @Name varchar(100),
		   @Value varchar(100),
           @Status tinyint,
           @UpdatedByContactId int
)
AS
BEGIN  
   UPDATE [dbo].[Security_Permission]
   SET 
      [Name] = @Name, 
	  [Value] = @Value,
      [Status] = @Status,
      [UpdatedDate] = SYSDATETIME(),
      [UpdatedByContactId] = @UpdatedByContactId
   
		
     WHERE Id = @Id
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Role_Delete]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Role_Delete](
            @RoleId  int
        
)
AS
BEGIN  
    DELETE FROM  [dbo].[Security_Role] WHERE Id = @RoleId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Role_GetAll]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Role_GetAll]

AS
BEGIN  
SELECT [Id]
      ,[Name]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_Role]

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Role_GetById]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Role_GetById]
@RoleId int
AS
BEGIN  
SELECT [Id]
      ,[Name]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_Role] Where Id = @RoleId

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Role_Insert]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Role_Insert]( 
           @Name varchar(100),
           @Status tinyint,
           @CreatedByContactId int,
           @ObjectId varchar(40)
)
AS
BEGIN  
INSERT INTO [dbo].[Security_Role]
           ([Name]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[ObjectId])
     VALUES
           (
		   @Name,
           @Status, 
           SYSDATETIME(), 
           @CreatedByContactId, 
           @ObjectId
		   )
END


GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Role_Update]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Role_Update](
           @Id  int,
           @Name varchar(100),
           @Status tinyint,
           @UpdatedByContactId int
)
AS
BEGIN  
   UPDATE [dbo].[Security_Role]
   SET 
      [Name] = @Name, 
      [Status] = @Status,
      [UpdatedDate] = SYSDATETIME(),
      [UpdatedByContactId] = @UpdatedByContactId
   
		
     WHERE Id = @Id
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_spContact_Delete]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_spContact_Delete](
            @ContactId  int
        
)
AS
BEGIN  
    DELETE FROM  [dbo].[Security_Contact] WHERE Id = @ContactId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_User_GetAll]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Aminul
-- Create date: 11 Jan 2021
-- Description:	Get All Users
-- =============================================
CREATE PROCEDURE [dbo].[SP_Security_User_GetAll]	
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
      ,[CreatedByContactId]
      ,[CreatedDate]
      ,[UpdatedByContactId]
	  ,[UpdatedDate] 
      ,[ObjectId]
  FROM [dbo].[Security_User]

END





GO
/****** Object:  StoredProcedure [dbo].[SP_Security_User_GetByUsername]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Aminul
-- Create date: 11 Jan 2021
-- Description:	Get All Users
-- =============================================
CREATE PROCEDURE [dbo].[SP_Security_User_GetByUsername]	
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





GO
/****** Object:  StoredProcedure [dbo].[SP_Security_User_Insert]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save User info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_User_Insert](
    @CompanyId AS INT,
    @ContactId AS INT,
	@RoleId AS INT,
	@Username AS NVARCHAR(100),
	@PasswordHash AS NVARCHAR(100),
	@PasswordSalt AS NVARCHAR(100),
	@UserGuid AS VARCHAR(100),
	@Status as int
)
AS
BEGIN  
           INSERT INTO [dbo].[Security_User]
           ([CompanyId]
		   ,[ContactId]
           ,[RoleId]
           ,[Username]
           ,[PasswordHash]
           ,[PasswordSalt]
           ,[ObjectId]
		   ,[Status]
		   ,[CreatedDate]
          )
     VALUES
           (@CompanyId
		   ,@ContactId
           ,@RoleId
           ,@Username
           ,@PasswordHash
           ,@PasswordSalt
           ,@UserGuid
		   ,@Status
		   , SYSDATETIME()
          )
END




GO
/****** Object:  StoredProcedure [dbo].[SP_Security_User_Update]    Script Date: 5/23/2022 6:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SP_Security_User_Update]	
    @UserId INT,
	@ContactId INT ,
    @FirstName NVARCHAR(150),
	@LastName NVARCHAR(150) ,
	@UserName NVARCHAR(100),
	@Email NVARCHAR(100) ,
	@CompanyId int ,
	@Phone NVARCHAR(20),
	@RoleId INT

AS
BEGIN

  UPDATE [dbo].[Security_User]   SET  [Username] = @UserName,   [RoleId] = @RoleId WHERE  Id = @UserId
  
  UPDATE Security_Contact SET CompanyId = @CompanyId,FirstName = @FirstName, LastName = @LastName,Email = ISNULL(@Email,''), Phone = @Phone WHERE Id = @ContactId

END

GO
USE [master]
GO
ALTER DATABASE [KowToMateERP] SET  READ_WRITE 
GO
