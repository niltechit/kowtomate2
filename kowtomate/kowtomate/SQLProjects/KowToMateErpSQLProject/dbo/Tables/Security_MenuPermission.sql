CREATE TABLE [dbo].[Security_MenuPermission] (
    [Id]                 INT          IDENTITY (1, 1) NOT NULL,
    [MenuId]             INT          NULL,
    [PermissionId]       INT          NULL,
    [UpdatedDate]        DATETIME     NOT NULL,
    [UpdatedByContactId] INT          NOT NULL,
    [ObjectId]           VARCHAR (40) NOT NULL,
    CONSTRAINT [PK_MenuPermission] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MenuPermission_MenuPermission] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[Security_MenuPermission] ([Id]),
    CONSTRAINT [FK_MenuPermission_MenuPermission1] FOREIGN KEY ([Id]) REFERENCES [dbo].[Security_MenuPermission] ([Id])
);

