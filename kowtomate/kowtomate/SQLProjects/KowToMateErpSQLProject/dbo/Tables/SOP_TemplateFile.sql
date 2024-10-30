CREATE TABLE [dbo].[SOP_TemplateFile] (
    [Id]                      INT           IDENTITY (1, 1) NOT NULL,
    [SOPTemplateId]           INT           NULL,
    [FileName]                VARCHAR (100) NULL,
    [FileType]                VARCHAR (20)  NULL,
    [ActualPath]              VARCHAR (300) NULL,
    [ModifiedPath]            VARCHAR (300) NULL,
    [IsDeleted]               BIT           NULL,
    [CreatedDate]             DATETIME      NULL,
    [CreatedByContactId]      INT           NULL,
    [UpdatedDate]             DATETIME      NULL,
    [UpdatedByContactId]      INT           NULL,
    [FileModifiedByContactId] INT           NULL,
    [ObjectId]                VARCHAR (40)  NULL,
    CONSTRAINT [PK_SOP_TemplateFile] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SOP_TemplateFile_SOP_Template] FOREIGN KEY ([SOPTemplateId]) REFERENCES [dbo].[SOP_Template] ([Id])
);

