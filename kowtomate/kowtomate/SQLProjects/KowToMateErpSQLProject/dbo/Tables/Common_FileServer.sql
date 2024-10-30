CREATE TABLE [dbo].[Common_FileServer] (
    [Id]                 SMALLINT      IDENTITY (1, 1) NOT NULL,
    [FileServerType]     TINYINT       NOT NULL,
    [Name]               VARCHAR (100) NOT NULL,
    [UserName]           VARCHAR (100) NULL,
    [Password]           VARCHAR (100) NULL,
    [AccessKey]          VARCHAR (100) NULL,
    [SecretKey]          VARCHAR (100) NULL,
    [RootFolder]         VARCHAR (150) NULL,
    [SshKeyPath]         VARCHAR (150) NULL,
    [Host]               VARCHAR (150) NULL,
    [Protocol]           VARCHAR (10)  NULL,
    [Status]             TINYINT       NULL,
    [CreatedDate]        DATETIME      NOT NULL,
    [CreatedByContactId] INT           NOT NULL,
    [UpdatedDate]        DATETIME      NULL,
    [UpdatedByContactId] INT           NULL,
    [ObjectId]           VARCHAR (40)  NOT NULL,
    [IsDefault] BIT NULL, 
    CONSTRAINT [PK_Common_FileServer] PRIMARY KEY CLUSTERED ([Id] ASC)
);

