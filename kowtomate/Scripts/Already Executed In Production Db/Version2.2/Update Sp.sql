GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetAssignOrderItemByContactIdAndTeamId]    Script Date: 5/16/2023 2:04:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Author:		Rakib
-- Create date:4/14/2023
-- Description:	Get An Editor all assigned image which exist in inproduction,distribute, reworkinproduction, reworkdistributation

-- =============================================
ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetAssignOrderItemByContactIdAndTeamId]
	@teamId int,
	@ContactId int
AS
BEGIN

	    SELECT a.OrderId into #oIds FROM  [dbo].[Order_Assigned_Team] a WITH(NOLOCK)
                    INNER JOIN Management_TeamMember m WITH(NOLOCK) ON m.TeamId = a.TeamId
                    WHERE a.TeamId = @teamId
		SELECT ci.*
		FROM [dbo].[Order_ClientOrderItem] as ci inner join dbo.Order_AssignedImageEditor as aie on ci.Id = aie.Order_ImageId where AssignContactId = @ContactId and Status in (7,8,9,11,12) and  ci.ClientOrderId in (select OrderId from #oIds)
		

END


alter table CompanyGeneralSettings add AllowAutoUploadFromEditorPc bit
alter table CompanyGeneralSettings add AllowAutoUploadFromQcPc bit
alter table CompanyGeneralSettings add AllowAutoDownloadToEditorPc bit
alter table CompanyGeneralSettings add AllowClientWiseImageProcessing bit


GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetAllIsSharedFolderEditorContact]    Script Date: 5/17/2023 3:32:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Create date: 29 March 2023
-- Description: SP_Security_Contact_GetAll
-- =============================================
ALTER PROCEDURE [dbo].[SP_Security_Contact_GetAllIsSharedFolderEditorContact]
AS
BEGIN  
  SET NOCOUNT ON;
  
  select cgs.CompanyId into #autoDistributedCompanyIds  from dbo.CompanyGeneralSettings cgs where cgs.AllowAutoUploadFromEditorPc =1
  
  select ct.TeamId into #teamIds from dbo.Common_CompanyTeam  ct where CompanyId in (select CompanyId from #autoDistributedCompanyIds)
  
  select mtm.ContactId into #autoUploadContactId from dbo.Management_TeamMember mtm where TeamId in (select TeamId from #teamIds)
 
 
  DECLARE @EditorRoleObjectId varchar (100)
  select @EditorRoleObjectId= ObjectId from Security_Role where Name = 'Editor'
  SELECT *
  FROM [dbo].[Security_Contact] where isSharedFolderEnable = 1 and Id in (  select ContactId from Security_User as su inner join Security_UserRole as sur on sur.UserObjectId=su.ObjectId where sur.RoleObjectId= @EditorRoleObjectId) and Id in (select ContactId from #autoUploadContactId)

 

END





USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].SP_Order_ClientOrderItem_GetByImageNameAndAllowClientProcessingEnableCompany]    Script Date: 5/17/2023 4:00:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rakib
-- Create date: 17 5 2023
-- Description:	Find Order Item SP_Order_ClientOrderItem_GetByImageNameAndAllowClientProcessingEnableCompany]
-- =============================================
Create PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetByImageNameAndAllowClientProcessingEnableCompany]
	(
		
		@FileNameWithoutExtension varchar(100)
		)
AS
BEGIN
   
   SET NOCOUNT ON;

   

   select ci.* From 
   Order_ClientOrderItem ci
   Inner Join dbo.CompanyGeneralSettings cgs on ci.CompanyId = cgs.CompanyId
   where 
   FileNameWithoutExtension=@FileNameWithoutExtension 
   

END



GO
/****** Object:  StoredProcedure [dbo].[SP_CompanyGeneralSettings_GetCompanyGeneralSettingsByCompanyId]    Script Date: 5/17/2023 5:15:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE [dbo].[SP_CompanyGeneralSettings_GetCompanyGeneralSettingsByCompanyId]
    @companyId int
AS
BEGIN
    SELECT *
    FROM CompanyGeneralSettings
    WHERE CompanyId = @companyId
END





GO


/****** Object:  Table [dbo].[Order_ClientOrderItemHistory]    Script Date: 5/18/2023 6:13:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Order_ClientOrderItemHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ClientOrderItemId] [bigint] NULL,
	[OrderPlacedDate] [datetime] NULL,
	[AssignedDate] [datetime] NULL,
	[AssignedForSupportDate] [datetime] NULL,
	[DistributedDate] [datetime] NULL,
	[InProductionDate] [datetime] NULL,
	[ProductionDoneDate] [datetime] NULL,
	[InQcDate] [datetime] NULL,
	[ReworkDistributedDate] [datetime] NULL,
	[ReworkInProductionDate] [datetime] NULL,
	[ReworkDoneDate] [datetime] NULL,
	[ReworkQcDate] [datetime] NULL,
	[ReadyToDeliverDate] [datetime] NULL,
	[DeliveredDate] [datetime] NULL,
	[RejectedDate] [datetime] NULL,
	[CompletedDate] [datetime] NULL,
 CONSTRAINT [PK_Order_ClientOrderItemHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Order_ClientOrderItemHistory]  WITH CHECK ADD  CONSTRAINT [FK__Order_Cli__Clien__4B2D1C3C] FOREIGN KEY([ClientOrderItemId])
REFERENCES [dbo].[Order_ClientOrderItem] ([Id])
GO

ALTER TABLE [dbo].[Order_ClientOrderItemHistory] CHECK CONSTRAINT [FK__Order_Cli__Clien__4B2D1C3C]
GO

GO

/****** Object:  Index [IX_Order_ClientOrderItemHistory_ItemId]    Script Date: 5/18/2023 6:34:22 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Order_ClientOrderItemHistory_ItemId] ON [dbo].[Order_ClientOrderItemHistory]
(
	[ClientOrderItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO






GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetAllIsSharedFolderEditorContact]    Script Date: 5/19/2023 9:41:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Create date: 29 March 2023
-- Description: SP_Security_Contact_GetAll
-- =============================================
ALTER PROCEDURE [dbo].[SP_Security_Contact_GetAllIsSharedFolderEditorContact]
AS
BEGIN  
  SET NOCOUNT ON;

 SELECT DISTINCT
    c.*
FROM
    [dbo].[Security_Contact] c 
    INNER JOIN [dbo].[Order_AssignedImageEditor] oie ON oie.AssignContactId = c.Id
    INNER JOIN [dbo].[Order_ClientOrderItem] oci ON oci.Id = oie.Order_ImageId
	Inner Join dbo.CompanyGeneralSettings cs on cs.CompanyId = oci.CompanyId
	where cs.AllowAutoUploadFromEditorPc = 1 and oci.Status IN (7,8,11,12) and c.isSharedFolderEnable = 1

END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetAllIsSharedFolderQcContact]    Script Date: 5/19/2023 1:09:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	 Rakib	
-- Create date: 29 March 2023
-- Description: SP_Security_Contact_GetAll
-- =============================================
ALTER PROCEDURE [dbo].[SP_Security_Contact_GetAllIsSharedFolderQcContact]
AS
BEGIN  
  SET NOCOUNT ON;

  select cgs.CompanyId into #autoDistributedCompanyIds  from dbo.CompanyGeneralSettings cgs where cgs.EnableOrderDeliveryToFtp =1
  
  select ct.TeamId into #teamIds from dbo.Common_CompanyTeam  ct where CompanyId in (select CompanyId from #autoDistributedCompanyIds)
  
  select mtm.ContactId into #autoUploadContactId from dbo.Management_TeamMember mtm where TeamId in (select TeamId from #teamIds)


  DECLARE @EditorRoleObjectId varchar (100)
  select @EditorRoleObjectId= ObjectId from Security_Role where Name = 'Qc'
  SELECT *
  FROM [dbo].[Security_Contact] where isSharedFolderEnable = 1 and Id in (  select ContactId from Security_User as su inner join Security_UserRole as sur on sur.UserObjectId=su.ObjectId where sur.RoleObjectId= @EditorRoleObjectId) and Id in (select ContactId from #autoUploadContactId)
END


