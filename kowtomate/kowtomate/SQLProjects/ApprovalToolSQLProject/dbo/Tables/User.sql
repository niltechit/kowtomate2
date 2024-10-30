CREATE TABLE [dbo].[User] (
    [Id]                    INT            IDENTITY (1, 1) NOT NULL,
    [ContactId]             INT            NOT NULL,
    [RoleId]                INT            NOT NULL,
    [Username]              NVARCHAR (100) NOT NULL,
    [ProfileImageUrl]       NVARCHAR (250) NULL,
    [PasswordHash]          NVARCHAR (100) NULL,
    [PasswordSalt]          NVARCHAR (100) NULL,
    [RegistrationToken]     NVARCHAR (50)  NULL,
    [PasswordResetToken]    NVARCHAR (50)  NULL,
    [Status]                INT            NOT NULL,
    [LastLoginDateUtc]      DATETIME       NULL,
    [LastLogoutDateUtc]     DATETIME       NULL,
    [LastPasswordChangeUtc] DATETIME       NULL,
    [CreateFromUserIp]      NVARCHAR (100) NULL,
    [CreatedDateUtc]        DATETIME       NULL,
    [ChangedDateUtc]        DATETIME       NULL,
    [UserGuid]              VARCHAR (40)   NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_User_Contact] FOREIGN KEY ([ContactId]) REFERENCES [dbo].[Contact] ([Id]),
    CONSTRAINT [FK_User_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id])
);



