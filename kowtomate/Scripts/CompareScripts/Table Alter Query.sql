GO

ALTER TABLE [dbo].[Accounting_OverheadCost] ADD  CONSTRAINT [DF__Accountin__Creat__583CFE97]  DEFAULT (getdate()) FOR [CreatedDate]
GO

ALTER TABLE [dbo].[HR_Designation]
ADD [DayOffMonday] BIT NULL,
    [DayOffTuesday] BIT NULL,
    [DayOffWednesday] BIT NULL,
    [DayOffThursday] BIT NULL,
    [DayOffFriday] BIT NULL,
    [DayOffSaturday] BIT NULL,
    [DayOffSunday] BIT NULL;


	GO

ALTER TABLE [dbo].[Common_Category] ADD  CONSTRAINT [DF__Common_Ca__IsDel__094A4A46]  DEFAULT ((0)) FOR [IsDeleted]
GO
GO

ALTER TABLE [dbo].[HR_Designation]
ADD [DayOffMonday] BIT NULL,
    [DayOffTuesday] BIT NULL,
    [DayOffWednesday] BIT NULL,
    [DayOffThursday] BIT NULL,
    [DayOffFriday] BIT NULL,
    [DayOffSaturday] BIT NULL,
    [DayOffSunday] BIT NULL;


	GO
ALTER TABLE HR_LeaveSubType
ADD [IsDeleted] BIT
GO


	GO
ALTER TABLE HR_LeaveType
ADD [IsDeleted] BIT
GO


ALTER TABLE [dbo].[Order_ClientOrderItem]
ADD [CategorySetStatus] TINYINT NULL,
    [CategoryApprovedByContactId] INT NULL,
    [ClientCategoryId] INT NULL,
    [CategorySetByContactId] INT NULL,
    [CategorySetDate] DATETIME NULL,
    [CategoryPrice] NVARCHAR(255) NULL,
    [TimeInMinute] DECIMAL(18, 2) NULL;

	GO

	ALTER TABLE [dbo].[Report_DynamicReportInfo]
ADD [PermissionObjectId] VARCHAR(40) NULL,
    [ReportCode] VARCHAR(100) NOT NULL,
    [ReportType] TINYINT NULL,
    [WhereClause] VARCHAR(MAX) NULL;


	GO
	ALTER TABLE [dbo].[Report_TableColumn]
ADD [BackgroundColorRules] VARCHAR(200) NULL,
    [DispalyFormat] VARCHAR(100) NULL,
    [Groupable] BIT DEFAULT ((0)) NOT NULL,
    [IsDefaultGroup] BIT DEFAULT ((0)) NOT NULL,
    [ShowGroupTotal] BIT DEFAULT ((0)) NOT NULL,
    [JoinInfoId] INT NULL,
    [SortingType] SMALLINT NULL,
    [JoinInfo2Id] INT NULL,
    [JoinInfo3Id] INT NULL,
    [JoinInfo4Id] INT NULL,
    [JoinInfo5Id] INT NULL;
