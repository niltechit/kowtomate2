GO
/****** Object:  StoredProcedure [dbo].[SP_GetTeamMembersWhoSupportAnotherTeamByTeamId]    Script Date: 5/23/2024 4:00:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Rakib	
-- Create date: 20-03-2024
-- Description:	SP_GetTeamMembersWhoSupportAnotherTeamByTeamId
-- =============================================
ALTER PROCEDURE [dbo].[SP_GetTeamMembersWhoSupportAnotherTeamByTeamId]
(
    @TeamId INT
)
AS
BEGIN
    SELECT c.Id, c.FirstName, c.LastName,c.DownloadFolderPath,c.IsUserActive,t.Name as TeamName,c.EmployeeId
    FROM [dbo].[Management_TeamMember] mt
    INNER JOIN [dbo].[Security_Contact] c ON mt.ContactId = c.Id
	Inner Join dbo.Management_Team t on t.Id = mt.TeamId
    WHERE mt.ContactId in (select ContactId where IsSupportingMember = 1 and TeamId != @TeamId)
    GROUP BY c.Id, c.FirstName, c.LastName,c.DownloadFolderPath,c.IsUserActive,t.Name,c.EmployeeId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_GetAvailableSupportMembersForProductionByTeamId]    Script Date: 5/23/2024 3:59:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Rakib	
-- Create date: 20-03-2024
-- Description:	SP_GetAvailableSupportMembersForProductionByTeamId

-- =============================================
ALTER PROCEDURE [dbo].[SP_GetAvailableSupportMembersForProductionByTeamId]
(
    @TeamId INT
)
AS
BEGIN
    SELECT c.Id, c.FirstName, c.LastName,c.DownloadFolderPath,c.IsUserActive,t.Name as TeamName,c.EmployeeId
    FROM [dbo].[Management_TeamMember] mt
    INNER JOIN [dbo].[Security_Contact] c ON mt.ContactId = c.Id
	Inner Join dbo.Management_Team t on t.Id = mt.TeamId
    WHERE mt.TeamId = @TeamId and mt.IsSupportingMember = 1
    GROUP BY c.Id, c.FirstName, c.LastName,c.DownloadFolderPath,c.IsUserActive,t.Name,c.EmployeeId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_GetAvailableTeamMembersForProductionByTeamId]    Script Date: 5/23/2024 3:55:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Rakib	
-- Create date: 20-03-2024
-- Description:	SP_GetAvailableTeamMebersForProductionByTeamId

-- =============================================
ALTER PROCEDURE [dbo].[SP_GetAvailableTeamMembersForProductionByTeamId]
(
    @TeamId INT
)
AS
BEGIN
    SELECT c.Id, c.FirstName, c.LastName,c.DownloadFolderPath,c.IsUserActive ,t.Name as TeamName,c.EmployeeId
    FROM [dbo].[Management_TeamMember] mt
    INNER JOIN [dbo].[Security_Contact] c ON mt.ContactId = c.Id
	Inner Join dbo.Management_Team t on t.Id = mt.TeamId
    WHERE mt.TeamId = @TeamId and mt.ContactId not in (select ContactId from Management_TeamMember where IsSupportingMember = 1)
    GROUP BY c.Id, c.FirstName, c.LastName,c.DownloadFolderPath,c.IsUserActive,t.Name,c.EmployeeId
END




GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderAssignedItem_GetByOrderId]    Script Date: 5/24/2024 2:03:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_Order_ClientOrderAssignedItem_GetByOrderId]
(
	@OrderId int,
	@ContactId int
	
)
as
begin
	SELECT orderitem.[Id]
      ,orderitem.[CompanyId]
      ,[ClientOrderId]
      ,[FileName]
      ,[ExteranlFileInputPath]
	  ,[InternalFileInputPath]
      ,[ExternalFileOutputPath]
      ,[InternalFileOutputPath]
	  ,[PartialPath]
      ,[UnitPrice]
      ,orderitem.[IsDeleted]
      ,orderitem.[CreatedDate]
      ,orderitem.[CreatedByContactId]
      ,orderitem.[UpdatedDate]
      ,orderitem.[UpdatedByContactId]
      ,orderitem.[ObjectId]
      ,orderitem.[FileSize] as FileSize
	  ,orderitem.TeamId
	  ,orderitem.Status as Status
	  ,orderitem.ExternalStatus
	  ,orderitem.ProductionFileByteString
	  ,orderitem.FileByteString
	  ,orderitem.FileGroup
	   ,orderitem.ExpectedDeliveryDate
	  ,assignorder.AssignDate as OrderAssignDate
	  ,contact.FirstName as EditorFirstName
	  ,contact.LastName as EditorLastName
	  ,contact.EmployeeId
	  ,orderitem.ProductionDoneFilePath
	  ,assignorder.AssignContactId
	  ,mt.Name as TeamName
	  ,orderitem.ArrivalTime
	from Order_ClientOrderItem as orderitem
	inner join Order_AssignedImageEditor as assignorder WITH(NOLOCK) on orderitem.Id = assignorder.Order_ImageId and assignorder.IsActive = 1 
	inner join dbo.Security_Contact as contact  WITH(NOLOCK) on contact.Id=assignorder.AssignContactId
	left join dbo.Management_Team as mt  WITH(NOLOCK) on mt.Id = orderitem.TeamId
	where orderitem.ClientOrderId=@OrderId and assignorder.AssignContactId = @ContactId and orderitem.FileGroup <> 4 --4 means ColorRef
end
