CREATE TABLE [dbo].[Security_CompanyTypePermission] (
    [Id]                 INT          IDENTITY (1, 1) NOT NULL,
    [CompanyType]        TINYINT      NOT NULL,
    [PermissionId]       SMALLINT     NOT NULL,
    [UpdatedDate]        DATETIME     NULL,
    [UpdatedByContactId] INT          NULL,
    [ObjectId]           VARCHAR (40) NOT NULL,
    CONSTRAINT [PK_CompanyTypePermission] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CompanyTypePermission_Permission] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Security_Permission] ([Id])
);

