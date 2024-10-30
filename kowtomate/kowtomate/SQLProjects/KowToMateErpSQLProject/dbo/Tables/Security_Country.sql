CREATE TABLE [dbo].[Security_Country] (
    [Id]                 INT           IDENTITY (1, 1) NOT NULL,
    [Name]               VARCHAR (100) NOT NULL,
    [Code]               VARCHAR (6)   NOT NULL,
    [ObjectId]           VARCHAR (40)  NOT NULL,
    [CreatedDate]        DATETIME      NULL,
    [CreatedByContactId] INT           NULL,
    [UpdatedDate]        DATETIME      NULL,
    [UpdatedByContactId] INT           NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Country]
    ON [dbo].[Security_Country]([Name] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Country_1]
    ON [dbo].[Security_Country]([Code] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Country_2]
    ON [dbo].[Security_Country]([ObjectId] ASC);

