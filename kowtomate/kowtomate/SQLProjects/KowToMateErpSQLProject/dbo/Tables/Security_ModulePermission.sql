CREATE TABLE [dbo].[Security_ModulePermission] (
    [Id]                 INT          IDENTITY (1, 1) NOT NULL,
    [ModuleId]           SMALLINT     NOT NULL,
    [PermissionId]       SMALLINT     NOT NULL,
    [UpdatedDate]        DATETIME     NULL,
    [UpdatedByContactId] INT          NULL,
    [ObjectId]           VARCHAR (40) NOT NULL,
    CONSTRAINT [PK_ModulePermission] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ModulePermission_Module] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Security_Module] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ModulePermission]
    ON [dbo].[Security_ModulePermission]([ObjectId] ASC);

