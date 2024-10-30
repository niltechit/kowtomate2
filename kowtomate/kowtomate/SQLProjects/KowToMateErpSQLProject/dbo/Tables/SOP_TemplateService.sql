CREATE TABLE [dbo].[SOP_TemplateService] (
    [Id]                   INT          IDENTITY (1, 1) NOT NULL,
    [SOPTemplateId]        INT          NOT NULL,
    [SOPStandardServiceId] SMALLINT     NOT NULL,
    [Status]               TINYINT      NOT NULL,
    [IsDeleted]            BIT          NOT NULL,
    [CreatedDate]          DATETIME     NOT NULL,
    [CreatedByContactId]   DATETIME     NOT NULL,
    [UpdatedDate]          DATETIME     NULL,
    [UpdatedByContactId]   INT          NULL,
    [ObjectId]             VARCHAR (40) NOT NULL,
    CONSTRAINT [PK_SOP_TemplateService] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SOP_TemplateService_SOP_StandardService] FOREIGN KEY ([SOPStandardServiceId]) REFERENCES [dbo].[SOP_StandardService] ([Id]),
    CONSTRAINT [FK_SOP_TemplateService_SOP_Template] FOREIGN KEY ([SOPTemplateId]) REFERENCES [dbo].[SOP_Template] ([Id])
);

