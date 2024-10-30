CREATE TABLE [dbo].[SOP_StandardService] (
    [Id]                 SMALLINT      IDENTITY (1, 1) NOT NULL,
    [Name]               VARCHAR (500) NOT NULL,
    [SortOrder]          BIT           NULL,
    [Status]             TINYINT       NOT NULL,
    [IsDeleted]          BIT           NOT NULL,
    [CreatedaDate]       DATETIME      NOT NULL,
    [CreatedByContactId] INT           NOT NULL,
    [UpdatedDate]        DATETIME      NULL,
    [UpdatedByContactId] INT           NULL,
    [ObjectId]           VARCHAR (40)  NOT NULL,
    CONSTRAINT [PK_SOP_StandardService] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_SOP_StandardService]
    ON [dbo].[SOP_StandardService]([Name] ASC);

