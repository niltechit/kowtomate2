CREATE TABLE [dbo].[Security_Role] (
    [Id]                 SMALLINT      IDENTITY (1, 1) NOT NULL,
    [Name]               VARCHAR (100) NOT NULL,
    [Status]             TINYINT       NOT NULL,
    [CreatedDate]        DATETIME      NOT NULL,
    [CreatedByContactId] INT           NOT NULL,
    [UpdatedDate]        DATETIME      NULL,
    [UpdatedByContactId] DATETIME      NULL,
    [ObjectId]           VARCHAR (40)  NOT NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Role]
    ON [dbo].[Security_Role]([Name] ASC);

