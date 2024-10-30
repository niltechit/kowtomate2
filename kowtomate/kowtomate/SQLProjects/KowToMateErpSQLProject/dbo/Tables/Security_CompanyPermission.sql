CREATE TABLE [dbo].[Security_CompanyPermission] (
    [Id]                 INT          IDENTITY (1, 1) NOT NULL,
    [CompanyId]          INT          NOT NULL,
    [PermissionId]       SMALLINT     NOT NULL,
    [UpdatedDate]        DATETIME     NULL,
    [UpdatedByContactId] INT          NULL,
    [ObjectId]           VARCHAR (40) NULL,
    CONSTRAINT [PK_CompanyPermission] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CompanyPermission_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Common_Company] ([Id]),
    CONSTRAINT [FK_CompanyPermission_Permission] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Security_Permission] ([Id])
);

