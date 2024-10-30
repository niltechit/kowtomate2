CREATE TABLE [dbo].[Role] (
    [Id]             INT          IDENTITY (1, 1) NOT NULL,
    [Name]           VARCHAR (50) NOT NULL,
    [ChangedDateUtc] DATETIME     NULL,
    [RoleGuid]       VARCHAR (32) NOT NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([Id] ASC)
);

