CREATE TABLE [dbo].[Company] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (100) NOT NULL,
    [Email]              NVARCHAR (50)  NULL,
    [Phone]              NVARCHAR (20)  NULL,
    [Status]             INT            NOT NULL,
    [CreatedByContactId] INT            NULL,
    [CreatedDateUtc]     DATETIME       NOT NULL,
    [ChangedDateUtc]     DATETIME       NULL,
    [FileRootFolderPath] NVARCHAR (100) NULL,
    CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([Id] ASC)
);

