CREATE TABLE [dbo].[Email_Template] (
    [Id]                 SMALLINT       IDENTITY (1, 1) NOT NULL,
    [SenderAccountId]    SMALLINT       NOT NULL,
    [Name]               VARCHAR (100)  NOT NULL,
    [AccessCode]         VARCHAR (100)  NULL,
    [FromEmailAddress]   VARCHAR (50)   NULL,
    [BCCEmailAddresses]  VARCHAR (50)   NULL,
    [Subject]            VARCHAR (200)  NULL,
    [Body]               NVARCHAR (MAX) NULL,
    [Status]             SMALLINT       NOT NULL,
    [CreatedDate]        DATETIME       NOT NULL,
    [CreatedByContactId] INT            NOT NULL,
    [UpdatedDate]        DATETIME       NULL,
    [UpdatedByContactId] INT            NULL,
    [ObjectId]           VARCHAR (40)   NOT NULL,
    CONSTRAINT [PK_Email_Template] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Email_Template]
    ON [dbo].[Email_Template]([Name] ASC);

