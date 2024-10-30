CREATE TABLE [dbo].[Common_Company] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (100) NOT NULL,
    [Code]               VARCHAR (6)    NOT NULL,
    [CompanyType]        TINYINT        NOT NULL,
    [Telephone]          VARCHAR (30)   NULL,
    [Address1]           VARCHAR (100)  NULL,
    [Address2]           VARCHAR (100)  NULL,
    [City]               VARCHAR (30)   NULL,
    [State]              VARCHAR (30)   NULL,
    [Zipcode]            VARCHAR (10)   NULL,
    [Country]            VARCHAR (50)   NULL,
    [Status]             TINYINT        NOT NULL,
    [CreatedDate]        DATETIME       NOT NULL,
    [CreatedByContactId] INT            NOT NULL,
    [UpdatedDate]        DATETIME       NULL,
    [UpdatedByContactId] INT            NULL,
    [ObjectId]           VARCHAR (40)   NULL,
    CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Company]
    ON [dbo].[Common_Company]([Code] ASC);

