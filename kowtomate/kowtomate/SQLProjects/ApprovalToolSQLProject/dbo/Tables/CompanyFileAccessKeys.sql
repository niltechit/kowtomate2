CREATE TABLE [dbo].[CompanyFileAccessKeys] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]          INT            NOT NULL,
    [FileAccessType]     INT            NOT NULL,
    [BucketName]         NVARCHAR (100) NULL,
    [AccessKey]          NVARCHAR (100) NULL,
    [SecretKey]          NVARCHAR (100) NULL,
    [FileRootPath]       NVARCHAR (200) NULL,
    [KeyGuid]            VARCHAR (32)   NULL,
    [CreatedByContactId] INT            NULL,
    [CreatedDateUtc]     DATETIME       NOT NULL,
    [ChangedDateUtc]     DATETIME       NULL,
    CONSTRAINT [PK_CompanyFileAccessKeys] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CompanyFileAccessKeys_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Company] ([Id])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'FTP, Google Cloud, AWS', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'CompanyFileAccessKeys', @level2type = N'COLUMN', @level2name = N'FileAccessType';

