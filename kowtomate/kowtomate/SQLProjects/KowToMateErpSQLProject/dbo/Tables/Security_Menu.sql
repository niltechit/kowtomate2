CREATE TABLE [dbo].[Security_Menu] (
    [Id]                 INT           IDENTITY (1, 1) NOT NULL,
    [Name]               VARCHAR (100) NOT NULL,
    [ParentId]           INT           NOT NULL,
    [Icon]               VARCHAR (50)  NOT NULL,
    [IsLeftMenu]         BIT           NOT NULL,
    [IsTopMenu]          BIT           NOT NULL,
    [IsExternalMenu]     BIT           NOT NULL,
    [MenuUrl]            VARCHAR (150) NULL,
    [Status]             INT           NULL,
    [CreatedDate]        DATETIME      NULL,
    [CreatedByContactId] INT           NULL,
    [UpdatedDate]        DATETIME      NOT NULL,
    [UpdatedByContactId] INT           NOT NULL,
    [ObjectId]           VARCHAR (40)  NULL,
    CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Menu_Menu] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Security_Menu] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Menu]
    ON [dbo].[Security_Menu]([Name] ASC);

