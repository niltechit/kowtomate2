CREATE TABLE [dbo].[Common_ActivityLog] (
    [Id]                 INT           IDENTITY (1, 1) NOT NULL,
    [ActivityLogFor]     TINYINT       NOT NULL,
    [PrimaryId]          INT           NOT NULL,
    [Description]        VARCHAR (500) NULL,
    [CreatedDate]        DATETIME      NOT NULL,
    [CreatedByContactId] INT           NOT NULL,
    [ObjectId]           VARCHAR (504) NOT NULL,
    CONSTRAINT [PK_Common_ActivityLog] PRIMARY KEY CLUSTERED ([Id] ASC)
);

