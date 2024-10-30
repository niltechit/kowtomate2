CREATE TABLE [dbo].[FileTracking] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]          INT            NULL,
    [SourceDrive]        NVARCHAR (30)  NULL,
    [ParentDirectory]    NVARCHAR (250) NULL,
    [BucketName]         NVARCHAR (50)  NULL,
    [ActionDate]         DATETIME2 (7)  NULL,
    [ActionType]         NVARCHAR (20)  NULL,
    [Attachment]         NVARCHAR (250) NULL,
    [Comments]           NVARCHAR (MAX) NULL,
    [MarkupImageUrl]     NVARCHAR (250) NULL,
    [NoOfFiles]          INT            NULL,
    [SelectedFileNames]  NVARCHAR (MAX) NULL,
    [Status]             INT            NULL,
    [CreatedByContactId] INT            NULL,
    [CreatedDateUtc]     DATETIME       NULL,
    CONSTRAINT [PK_FileTracking] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_FileTracking_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Company] ([Id])
);

