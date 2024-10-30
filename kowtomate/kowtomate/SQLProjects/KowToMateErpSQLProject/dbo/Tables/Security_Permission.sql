CREATE TABLE [dbo].[Security_Permission] (
    [Id]                 SMALLINT      IDENTITY (1, 1) NOT NULL,
    [Name]               VARCHAR (100) NOT NULL,
    [Value]              VARCHAR (100) NOT NULL,
    [Status]             TINYINT       NOT NULL,
    [CreatedDate]        DATETIME      NOT NULL,
    [CreatedByContactId] INT           NOT NULL,
    [UpdatedDate]        DATETIME      NULL,
    [UpdatedByContactId] INT           NULL,
    [ObjectId]           VARCHAR (40)  NOT NULL,
    CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Permission]
    ON [dbo].[Security_Permission]([Name] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Permission_2]
    ON [dbo].[Security_Permission]([Value] ASC);

