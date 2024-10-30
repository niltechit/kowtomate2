GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetAssignOrderItemByContactIdAndTeamId]    Script Date: 5/2/2023 10:19:36 PM ******/
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
		FROM [dbo].[Order_ClientOrderItem] as ci inner join dbo.Order_AssignedImageEditor as aie on ci.Id = aie.Order_ImageId where AssignContactId = @ContactId and Status in (7,8,11,12) and  ci.ClientOrderId in (select OrderId from #oIds)
		




END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetAllByTeamId]    Script Date: 5/2/2023 7:20:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Rakib	
-- Create date: 28-04-2023
-- Description:	Get Contact By Team Id

-- =============================================
ALTER PROCEDURE [dbo].[SP_Security_Contact_GetAllByTeamId]
(
	@TeamId int
)
AS
BEGIN  
	SELECT c.Id, c.CompanyId,
	c.FirstName,
	c.LastName,
	c.DesignationId,
	c.Email,
	c.Phone,
	c.Phone,
	c.ProfileImageUrl,
	c.Status,
	c.CreatedDate,
	c.CreatedByContactId,
	c.UpdatedByContactId,
	c.ObjectId,
	c.EmployeeId,
	c.IsEmployee,
	c.DownloadFolderPath,
	c.IsUserActive,
	c.isSharedFolderEnable
  FROM [dbo].[Security_Contact] c WITH(NOLOCK)
  INNER JOIN dbo.Management_TeamMember as mt on mt.ContactId = c.Id
  where mt.TeamId = @TeamId
  
 
END


GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetDistributedAssignOrderItemByContactIdAndTeamId]    Script Date: 5/3/2023 1:58:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rakib
-- Create date: 22 March 2023
-- Description:	Get SP_Order_ClientOrderItem_GetDistributedAssignOrderItemByContactIdAndTeamId
-- =============================================
Create PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetDistributedAssignOrderItemByContactIdAndTeamId]
(
	@assignContactId int,
	@orderId bigint
)
as
begin
	SELECT *
FROM Order_ClientOrderItem
WHERE Id IN (
  SELECT oc.Id
  FROM Order_ClientOrderItem AS oc
  INNER JOIN Order_AssignedImageEditor AS oa ON oa.Order_ImageId = oc.Id
  WHERE oa.AssignContactId = @assignContactId
    AND oa.OrderId = @orderId and Status in (7,11) 
);
end