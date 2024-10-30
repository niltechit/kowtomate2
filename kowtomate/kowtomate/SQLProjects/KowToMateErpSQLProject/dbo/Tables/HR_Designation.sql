CREATE TABLE [dbo].[HR_Designation] (
    [Id]                 INT           IDENTITY (1, 1) NOT NULL,
    [Name]               VARCHAR (100) NOT NULL,
    [Status]             TINYINT       NOT NULL,
    [CreatedDate]        DATETIME      NOT NULL,
    [CreatedByContactId] INT           NOT NULL,
    [UpdatedDate]        DATETIME      NULL,
    [UpdatedByContactId] INT           NULL,
    [ObjectId]           VARCHAR (40)  NOT NULL,
    CONSTRAINT [PK_Designation] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Designation]
    ON [dbo].[HR_Designation]([Name] ASC);

