CREATE TABLE [dbo].[SOP_Template] (
    [Id]                             INT             IDENTITY (1, 1) NOT NULL,
    [CompanyId]                      INT             NOT NULL,
    [FileServerId]                   SMALLINT        NOT NULL,
    [Name]                           VARCHAR (100)   NOT NULL,
    [Version]                        SMALLINT        NOT NULL,
    [ParentTemplateId]               INT             NULL,
    [Instruction]                    NVARCHAR (MAX)  NULL,
    [UnitPrice]                      DECIMAL (10, 2) NULL,
    [Status]                         TINYINT         NOT NULL,
    [IsDeleted]                      BIT             NOT NULL,
    [CreatedDate]                    DATETIME        NOT NULL,
    [CreatedByContactId]             INT             NOT NULL,
    [UpdatedDate]                    DATETIME        NULL,
    [UpdatedByContactId]             INT             NULL,
    [InstructionModifiedByContactId] INT             NULL,
    [ObjectId]                       VARCHAR (40)    NOT NULL,
    CONSTRAINT [PK_SOP_Template] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SOP_Template_Common_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Common_Company] ([Id]),
    CONSTRAINT [FK_SOP_Template_Common_FileServer] FOREIGN KEY ([FileServerId]) REFERENCES [dbo].[Common_FileServer] ([Id])
);

