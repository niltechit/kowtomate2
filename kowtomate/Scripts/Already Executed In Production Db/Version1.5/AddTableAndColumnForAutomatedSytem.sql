alter table Client_ClientOrderFtp add  OutputRootFolder varchar(max)
alter table Client_ClientOrderFtp add  InputRootFolder varchar(max)

alter table Security_Contact add  IsUserActive bit

CREATE TABLE CompanyGeneralSettings (
    AutoAssignOrderToTeam BIT,
    AutoDistributeToEditor BIT,
    AutoQcPass BIT,
    AutoOperationPass BIT,
    AllowPartialDelivery BIT,
    EnableFtpOrderPlacement BIT,
    EnableOrderDeliveryToFtp BIT,
	AllowSingleOrderFromFTP BIT
);



GO
/****** Object:  StoredProcedure [dbo].[SP_Management_ActiveTeamMember_GetListWithDetails]    Script Date: 3/21/2023 7:20:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


Create PROCEDURE [dbo].[SP_Management_ActiveTeamMember_GetListWithDetails]
	@TeamId int
AS
BEGIN  
	


	SELECT TM.[Id]
		,TM.[ContactId]
		,CO.[FirstName]
		,CO.[LastName]
		,CO.[Phone]
		,TM.[TeamId]
		,TM.[TeamRoleId]
		,TR.[Name] AS TeamRoleName
		,TM.[CreatedDate]
		,TM.[CreatedByContactId]
		,TM.[UpdatedDate]
		,TM.[UpdatedByContactId]
		,TM.[ObjectId]

	FROM [dbo].[Management_TeamMember] AS TM WITH(NOLOCK)
	INNER JOIN [dbo].[Management_TeamRole] AS TR WITH(NOLOCK) ON TM.TeamRoleId = TR.Id
	INNER JOIN [dbo].[Security_Contact] AS CO WITH(NOLOCK) ON TM.ContactId = CO.Id
	
	WHERE TM.[TeamId] = @TeamId and TM.TeamRoleId = 2 and IsUserActive = 1 order by Id desc -- Need To Talk Aminul Vai

END



ALTER TABLE CompanyGeneralSettings ADD CompanyId int FOREIGN KEY (CompanyId) REFERENCES Common_Company(id)


GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetByAssignedContactIdAndOrderId]    Script Date: 4/10/2023 3:31:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rakib
-- Create date: 22 March 2023
-- Description:	Get SP_Order_ClientOrderItem_GetByAssignedContactIdAndOrderId
-- =============================================
Create PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetByAssignedContactIdAndOrderId]
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
    AND oa.OrderId = @orderId
);
end





GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetAllByOrderId]    Script Date: 3/25/2023 4:43:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:	Rakib	
-- Create date: 25 March 2023
-- Description:	Get All Order Item by Order Id
-- =============================================

Create PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetOrderItemsByOrderId]
	@OrderId int
AS
BEGIN  
	
	
	SELECT *
    from Order_ClientOrderItem
   where ClientOrderId=@OrderId and (IsDeleted=0 or IsDeleted is null) 


END

