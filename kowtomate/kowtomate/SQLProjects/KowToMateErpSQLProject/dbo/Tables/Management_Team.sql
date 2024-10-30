CREATE TABLE [dbo].[Management_Team] (
    [Id]                 INT          IDENTITY (1, 1) NOT NULL,
    [CompanyId]          INT          NULL,
    [Name]               VARCHAR (50) NULL,
    [CreatedDate]        DATETIME     NULL,
    [CreatedByContactid] INT          NULL,
    [UpdatedDate]        DATETIME     NULL,
    [UpdatedDateById]    INT          NULL,
    [ObjectId]           VARCHAR (40) NULL,
    CONSTRAINT [PK_Management_Team] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Management_Team_Common_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Common_Company] ([Id])
);

