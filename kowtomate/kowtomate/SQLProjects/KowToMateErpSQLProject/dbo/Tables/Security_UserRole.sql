CREATE TABLE [dbo].[Security_UserRole] (
    [Id]                 INT          IDENTITY (1, 1) NOT NULL,
    [UserId]             INT          NOT NULL,
    [RoleId]             SMALLINT     NOT NULL,
    [UpdatedDate]        DATETIME     NOT NULL,
    [UpdatedByContactId] INT          NOT NULL,
    [ObjectId]           VARCHAR (40) NOT NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserRole_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Security_Role] ([Id]),
    CONSTRAINT [FK_UserRole_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Security_User] ([Id])
);

