/****** Object:  StoredProcedure [dbo].[SP_GetContactWithMinImageStatus]    Script Date: 5/10/2023 3:07:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Rakib	
-- Create date: 10-05-2023
-- Description:	SP_GetContactWithMinImageStatus

-- =============================================


Alter PROCEDURE [dbo].[SP_GetContactWithMinImageStatus]
(
    @TeamId INT
)
AS
BEGIN
    SELECT c.Id, c.FirstName, c.LastName,c.DownloadFolderPath,c.IsUserActive , MIN(oci.Status) as Status
    FROM [dbo].[Management_TeamMember] mt
    INNER JOIN [dbo].[Security_Contact] c ON mt.ContactId = c.Id
    Left JOIN [dbo].[Order_AssignedImageEditor] oie ON oie.AssignContactId = c.Id
    left JOIN [dbo].[Order_ClientOrderItem] oci ON oci.Id = oie.Order_ImageId
    WHERE mt.TeamId = @TeamId
    GROUP BY c.Id, c.FirstName, c.LastName,c.DownloadFolderPath,c.IsUserActive
END




GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetAssignOrderItemByContactIdAndTeamId]    Script Date: 5/11/2023 1:03:39 PM ******/
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
		SELECT ci.FileName,ci.Status,ci.Id,ci.ClientOrderId,ci.FileGroup,ci.ExternalStatus
		FROM [dbo].[Order_ClientOrderItem] as ci inner join dbo.Order_AssignedImageEditor as aie on ci.Id = aie.Order_ImageId where AssignContactId = @ContactId and Status in (7,8,9,11,12) and  ci.ClientOrderId in (select OrderId from #oIds)
		

END



Alter table Order_AssignedImageEditor add  InQcAcknowledgeBy int null
Alter table Order_AssignedImageEditor add  InQcDoneBy int null
Alter table Order_AssignedImageEditor add  InQcAcknowledgedDate datetime null
Alter table Order_AssignedImageEditor add  InQcDoneDate datetime null



alter table CompanyGeneralSettings add IsIbrProcessedEnabled bit






