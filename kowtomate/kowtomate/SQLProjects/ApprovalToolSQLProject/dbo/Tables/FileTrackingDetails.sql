CREATE TABLE [dbo].[FileTrackingDetails] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [FileTrackingId] INT            NULL,
    [FileName]       NVARCHAR (200) NULL,
    [FilePathUrl]    NVARCHAR (MAX) NULL,
    [FileType]       NVARCHAR (30)  NULL,
    [FileSize]       BIGINT         NULL,
    [FileMarkUp]     NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_FileTrackingInfos] PRIMARY KEY CLUSTERED ([Id] ASC)
);

