CREATE TABLE [dbo].[Management_TeamRole] (
    [Id]                 INT          IDENTITY (1, 1) NOT NULL,
    [CompanyId]          INT          NULL,
    [Name]               VARCHAR (50) NULL,
    [CreatedDate]        DATETIME     NOT NULL,
    [CreatedByContactid] INT          NOT NULL,
    [UpdatedDate]        DATETIME     NULL,
    [UpdatedDateById]    INT          NULL,
    [ObjectId]           VARCHAR (40) NOT NULL,
    CONSTRAINT [PK_Management_TeamRole] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Management_TeamRole_Common_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Common_Company] ([Id])
);

