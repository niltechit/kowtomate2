CREATE TABLE [dbo].[Security_RolePermission] (
    [Id]                 INT          IDENTITY (1, 1) NOT NULL,
    [RoleId]             SMALLINT     NOT NULL,
    [PermissionId]       SMALLINT     NOT NULL,
    [UpdatedDate]        DATETIME     NULL,
    [UpdatedByContactId] INT          NULL,
    [ObjectId]           VARCHAR (40) NOT NULL,
    CONSTRAINT [PK_RolePermission] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RolePermission_Permission] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Security_Permission] ([Id]),
    CONSTRAINT [FK_RolePermission_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Security_Role] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_RolePermission]
    ON [dbo].[Security_RolePermission]([ObjectId] ASC);

