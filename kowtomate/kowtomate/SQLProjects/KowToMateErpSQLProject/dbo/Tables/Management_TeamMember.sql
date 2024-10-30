CREATE TABLE [dbo].[Management_TeamMember] (
    [Id]                 INT          IDENTITY (1, 1) NOT NULL,
    [UserId]             INT          NULL,
    [TeamId]             INT          NULL,
    [TeamRoleId]         INT          NULL,
    [UpdatedDate]        DATETIME     NULL,
    [UpdatedByContactId] INT          NULL,
    [ObjectId]           VARCHAR (40) NOT NULL,
    CONSTRAINT [PK_Management_TeamMember] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Management_TeamMember_Management_Team] FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Management_Team] ([Id]),
    CONSTRAINT [FK_Management_TeamMember_Management_TeamRole] FOREIGN KEY ([TeamRoleId]) REFERENCES [dbo].[Management_TeamRole] ([Id]),
    CONSTRAINT [FK_Management_TeamMember_Security_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Security_User] ([Id])
);

