CREATE TABLE [dbo].[ClientCategoryBaseRule] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]          INT            NOT NULL,
    [Name]               NVARCHAR (255) NOT NULL,
    [ClientCategoryId]   INT            NOT NULL,
    [IsActive]           BIT            NULL,
    [CreatedDate]        DATETIME       NULL,
    [CreatedByContactId] INT            NULL,
    [UpdatedDate]        DATETIME       NULL,
    [UpdatedByContactId] INT            NULL,
    [IsDeleted]          BIT            NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [idx_unique_CompanyId_ClientCategoryId_Name]
    ON [dbo].[ClientCategoryBaseRule]([CompanyId] ASC, [ClientCategoryId] ASC, [Name] ASC);
GO

CREATE TABLE [dbo].[ClientCategoryChangeLog] (
    [ClientCategoryId]       INT      NULL,
    [CategorySetByContactId] INT      NULL,
    [CategorySetDate]        DATETIME NULL,
    [ClientOrderItemId]      BIGINT   NULL,
    [Id]                     INT      IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[ClientCategoryRule] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [Name]                     NVARCHAR (255) NOT NULL,
    [ClientCategoryBaseRuleId] INT            NOT NULL,
    [Indicator]                NVARCHAR (255) NULL,
    [FilterType]               TINYINT        NULL,
    [ExecutionOrder]           DECIMAL (5, 2) NULL,
    [IsActive]                 BIT            NULL,
    [CreatedDate]              DATETIME       NULL,
    [UpdatedDate]              DATETIME       NULL,
    [IsDeleted]                BIT            NULL,
    [CreatedByContactId]       INT            NULL,
    [UpdatedByContactId]       INT            NULL,
    [Label]                    INT            NULL,
    CONSTRAINT [PK__ClientCa__3214EC072346F0F5] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [idx_unique__ClientCategoryRule_CompanyId_ClientCategoryId_Name]
    ON [dbo].[ClientCategoryRule]([ClientCategoryBaseRuleId] ASC, [Name] ASC);
GO

CREATE TABLE [dbo].[Report_JoinInfo] (
    [Id]                  INT           IDENTITY (1, 1) NOT NULL,
    [DynamicReportInfoId] INT           NOT NULL,
    [JoinName]            VARCHAR (150) NOT NULL,
    [JoinScript]          VARCHAR (MAX) NOT NULL,
    [DisplayOrder]        INT           NOT NULL,
    CONSTRAINT [PK_Report_JoinInfo] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[UI_GridVewFilter] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [GridViewSetupId] INT            NOT NULL,
    [Name]            NVARCHAR (100) NOT NULL,
    [FilterJson]      NVARCHAR (MAX) NOT NULL,
    [IsDefault]       BIT            NOT NULL,
    [UpdatedDate]     DATETIME       NOT NULL,
    [LogicalOperator] VARCHAR (5)    NULL,
    [SortColumn]      VARCHAR (50)   NULL,
    [SortOrder]       VARCHAR (10)   NULL,
    [IsPublic]        BIT            NOT NULL,
    [ContactId]       INT            NOT NULL,
    CONSTRAINT [PK_UI_GridVewFilter] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UI_GridVewFilter_Security_Contact] FOREIGN KEY ([ContactId]) REFERENCES [dbo].[Security_Contact] ([Id]),
    CONSTRAINT [FK_UI_GridVewFilter_UI_GridViewSetup] FOREIGN KEY ([GridViewSetupId]) REFERENCES [dbo].[UI_GridViewSetup] ([Id]),
    CONSTRAINT [IX_UI_GridVewFilter_SetupIdAndName] UNIQUE NONCLUSTERED ([Name] ASC, [GridViewSetupId] ASC),
    CONSTRAINT [UC_UI_GridVewFilter_NameAndContactAndIsPublic] UNIQUE NONCLUSTERED ([GridViewSetupId] ASC, [Name] ASC)
);
GO

CREATE TABLE [dbo].[UI_GridViewSetup] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [ContactId]           INT            NOT NULL,
    [Name]                VARCHAR (100)  NOT NULL,
    [OrderByColumn]       VARCHAR (100)  NULL,
    [OrderDirection]      VARCHAR (100)  NULL,
    [IsDefault]           BIT            NOT NULL,
    [CreatedDate]         DATETIME       NOT NULL,
    [CreatedByContactId]  INT            NOT NULL,
    [UpdatedDate]         DATETIME       NULL,
    [UpdatedByContactId]  INT            NULL,
    [ObjectId]            VARCHAR (40)   NOT NULL,
    [GridViewFor]         SMALLINT       DEFAULT ((1)) NULL,
    [IsPublic]            BIT            NOT NULL,
    [ViewStateJson]       NVARCHAR (MAX) NULL,
    [DynamicReportInfoId] INT            NULL,
    CONSTRAINT [PK_UI_GridViewSetup] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UI_GridViewSetup_Security_Contact] FOREIGN KEY ([ContactId]) REFERENCES [dbo].[Security_Contact] ([Id]),
    CONSTRAINT [UC_UI_GridViewSetup_NameAndContactId] UNIQUE NONCLUSTERED ([Name] ASC, [ContactId] ASC)
);
GO

CREATE TABLE [dbo].[UI_GridViewSetupColumn] (
    [Id]              INT        IDENTITY (1, 1) NOT NULL,
    [ColoumnId]       INT        NOT NULL,
    [GridViewSetupId] INT        NOT NULL,
    [DisplayOrder]    INT        DEFAULT ((1)) NOT NULL,
    [Width]           FLOAT (53) DEFAULT ((160)) NOT NULL,
    CONSTRAINT [PK_UI_GridViewSetupColumn] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UI_GridViewSetupColumn_UI_GridViewSetup] FOREIGN KEY ([GridViewSetupId]) REFERENCES [dbo].[UI_GridViewSetup] ([Id]),
    CONSTRAINT [IX_UI_GridViewSetupColumn_IdAndCol] UNIQUE NONCLUSTERED ([ColoumnId] ASC, [GridViewSetupId] ASC)
);
GO

