CREATE TABLE [dbo].[Security_Module] (
    [Id]                 SMALLINT      IDENTITY (1, 1) NOT NULL,
    [Name]               VARCHAR (100) NOT NULL,
    [Status]             TINYINT       NOT NULL,
    [CreatedDate]        DATETIME      NOT NULL,
    [CreatedByContactId] INT           NOT NULL,
    [UpdatedDate]        DATETIME      NULL,
    [UpdatedByContactId] INT           NULL,
    [ObjectId]           INT           NOT NULL,
    CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Module]
    ON [dbo].[Security_Module]([Name] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Module_1]
    ON [dbo].[Security_Module]([ObjectId] ASC);

