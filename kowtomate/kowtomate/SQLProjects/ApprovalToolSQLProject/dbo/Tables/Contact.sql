CREATE TABLE [dbo].[Contact] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]      INT            NULL,
    [FirstName]      NVARCHAR (150) NOT NULL,
    [LastName]       NVARCHAR (150) NULL,
    [Email]          NVARCHAR (100) NOT NULL,
    [Phone]          NVARCHAR (20)  NULL,
    [ImageUrl]       NVARCHAR (350) NULL,
    [ContactGuid]    VARCHAR (40)   NULL,
    [CreatedDateUtc] DATETIME       NOT NULL,
    [ChangedDateUtc] DATETIME       NULL,
    CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Contact_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Company] ([Id])
);



