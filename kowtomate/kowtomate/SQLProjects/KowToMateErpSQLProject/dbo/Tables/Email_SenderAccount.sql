CREATE TABLE [dbo].[Email_SenderAccount] (
    [Id]                    SMALLINT      IDENTITY (1, 1) NOT NULL,
    [Name]                  VARCHAR (100) NOT NULL,
    [Email]                 VARCHAR (100) NULL,
    [EmailDisplayName]      VARCHAR (50)  NULL,
    [MailServer]            VARCHAR (50)  NULL,
    [Port]                  SMALLINT      NULL,
    [UserName]              VARCHAR (100) NULL,
    [Password]              VARCHAR (100) NULL,
    [ApiKey]                VARCHAR (100) NULL,
    [SecretKey]             VARCHAR (100) NULL,
    [Domain]                VARCHAR (100) NULL,
    [EnableSSL]             BIT           NOT NULL,
    [UseDefaultCredentials] BIT           NOT NULL,
    [IsDefault]             BIT           NOT NULL,
    [Status]                TINYINT       NOT NULL,
    [CreatedDate]           DATETIME      NOT NULL,
    [CreatedByContactId]    INT           NOT NULL,
    [UpdatedDate]           DATETIME      NULL,
    [UpdatedByContactId]    INT           NULL,
    [ObjectId]              VARCHAR (40)  NOT NULL,
    CONSTRAINT [PK_Email_SenderAccount] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Email_SenderAccount]
    ON [dbo].[Email_SenderAccount]([Name] ASC);

