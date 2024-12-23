USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Management_TeamMember_GetListWithDetails]    Script Date: 6/9/2023 3:29:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



ALTER PROCEDURE [dbo].[SP_Security_Contact_GetListWithDetailsForCompanyWise]
(
	@companyId int
)
AS
BEGIN  
	SELECT c.[Id]     
	  ,com.[Name] CompanyName
	  ,com.[Id] CompanyId
      ,c.[FirstName]
      ,c.[LastName]
	  ,ISNULL(u.[Id], 0) AS UserId
	  ,u.[Username] AS UserName
	  ,u.[ObjectId] AS UserObjectId
	  ,d.[Name] DesignationName
      ,c.[Email]
      ,c.[Phone]
      ,c.[Status]
      ,c.[CreatedDate]
      ,c.[CreatedByContactId]
      ,c.[ObjectId]
	  ,c.[EmployeeId]
  FROM [dbo].[Security_Contact] c WITH(NOLOCK)
  INNER JOIN [dbo].Common_Company com on com.Id = c.CompanyId
  LEFT JOIN [dbo].[HR_Designation] d on d.Id = c.DesignationId  
  LEFT JOIN [dbo].[Security_User] u on u.ContactId = c.Id
  where com.Id=@companyId order by c.EmployeeId asc

END

GO
-- =============================================
-- Author:	Md Zakir Hossain	
-- Create date: 28/09/2022
-- Description:	Fetching Contact Info By Company Object ID
-- [dbo].[SP_Security_Contact_GetAllByCompanyObjectId] '412379d6-ccb3-46ff-a2ed-c86f15e321c0'
-- =============================================
ALTER PROCEDURE [dbo].[SP_Security_Contact_GetAllByCompanyObjectId]
(
	@ObjectId varchar(40)
)
AS
BEGIN  
	SELECT c.[Id]     
	  ,com.[Name] CompanyName
	  ,com.[Id] CompanyId
      ,c.[FirstName]
      ,c.[LastName]
	  ,ISNULL(u.[Id], 0) AS UserId
	  ,u.[Username] AS UserName
	  ,u.[ObjectId] AS UserObjectId
	  ,d.[Name] DesignationName
      ,c.[Email]
      ,c.[Phone]
      ,c.[Status]
      ,c.[CreatedDate]
      ,c.[CreatedByContactId]
      ,c.[ObjectId]
	  ,c.[EmployeeId]
  FROM [dbo].[Security_Contact] c WITH(NOLOCK)
  INNER JOIN [dbo].Common_Company com on com.Id = c.CompanyId
  LEFT JOIN [dbo].[HR_Designation] d on d.Id = c.DesignationId  
  LEFT JOIN [dbo].[Security_User] u on u.ContactId = c.Id
  where com.ObjectId = @ObjectId order by c.EmployeeId asc
END

GO

-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

ALTER PROCEDURE [dbo].[SP_Management_TeamMember_GetAll]

AS
BEGIN  
	SELECT  mt.Id
      ,mt.ContactId
	  ,sc.EmployeeId
	  ,sc.FirstName
	  ,sc.LastName
	  ,sc.Phone
	  ,mt.TeamId
	  ,mt.TeamRoleId
	  ,tr.Name as TeamRoleName
	  ,mt.CreatedDate
	  ,mt.CreatedByContactId
	  ,mt.UpdatedDate
	  ,mt.UpdatedByContactId
	  ,mt.ObjectId
	  ,t.Name as TeamName
	  
	FROM [dbo].[Management_TeamMember] as mt
	Inner join dbo.Security_Contact as sc
	on mt.ContactId = sc.Id
	Inner join 
	dbo.Management_TeamRole as tr 
	on tr.Id = mt.TeamRoleId
	Inner join dbo.Management_Team as t 
	on t.Id = mt.TeamId
	ORDER BY sc.EmployeeId asc
END

GO

ALTER PROCEDURE [dbo].[SP_Management_TeamMember_GetListWithDetails]
	@TeamId int
AS
BEGIN  
	


	SELECT TM.[Id]
		,TM.[ContactId]
		,CO.EmployeeId
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
	
	WHERE TM.[TeamId] = @TeamId and TM.TeamRoleId = 2 order by Id desc -- Need To Talk Aminul Vai

END

GO

ALTER PROCEDURE [dbo].[SP_Management_TeamMember_GetListWithDetails]
	@TeamId int
AS
BEGIN  
	


	SELECT TM.[Id]
		,TM.[ContactId]
		,CO.EmployeeId
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
	
	WHERE TM.[TeamId] = @TeamId and TM.TeamRoleId = 2 order by Id desc -- Need To Talk Aminul Vai

END