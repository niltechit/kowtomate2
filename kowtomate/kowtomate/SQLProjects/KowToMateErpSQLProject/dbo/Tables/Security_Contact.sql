CREATE TABLE [dbo].[Security_Contact] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]          INT            NOT NULL,
    [FirstName]          NVARCHAR (100) NULL,
    [LastName]           NVARCHAR (100) NULL,
    [Designation]        INT            NOT NULL,
    [Email]              NVARCHAR (100) NULL,
    [Phone]              VARCHAR (20)   NULL,
    [ProfileImageUrl]    VARCHAR (200)  NULL,
    [Status]             INT            NOT NULL,
    [CreatedDate]        DATETIME       NOT NULL,
    [CreatedByContactId] INT            NOT NULL,
    [UpdatedDate]        DATETIME       NULL,
    [UpdatedByContactId] INT            NULL,
    [ObjectId]           VARCHAR (40)   NOT NULL,
    CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Contact_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Common_Company] ([Id]),
    CONSTRAINT [FK_Contact_Designation] FOREIGN KEY ([Designation]) REFERENCES [dbo].[HR_Designation] ([Id])
);

