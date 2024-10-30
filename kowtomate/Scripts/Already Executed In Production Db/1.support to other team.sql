GO
/****** Object:  StoredProcedure [dbo].[SP_Management_TeamMember_GetListWithDetails]    Script Date: 3/21/2024 11:39:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
	INNER JOIN [dbo].[Security_Contact] AS CO WITH(NOLOCK) ON TM.ContactId = CO.Id AND CO.IsDeleted=0
	
	WHERE TM.[TeamId] = @TeamId
	 and TM.TeamRoleId = 2
	 and TM.ContactId not in (select ContactId from Management_TeamMember where IsSupportingMember = 1)
	 or (TM.[TeamId] = @TeamId and TM.ContactId in (select ContactId from Management_TeamMember where IsSupportingMember = 1))
	  order by Id desc -- Need To Talk Aminul Vai

END



GO
/****** Object:  StoredProcedure [dbo].[SP_CancelSupportingMemberByTeamIdandContactId]    Script Date: 3/21/2024 10:07:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Rakib	
-- Create date: 20-03-2024
-- Description:	SP_CancelSupportingMemberByTeamIdandContactId

-- =============================================
Alter PROCEDURE [dbo].[SP_CancelSupportingMemberByTeamIdandContactId]
(
    @TeamId INT,
	@ContactId INT
)
AS
BEGIN
    delete from Management_TeamMember where TeamId = @TeamId and ContactId = @ContactId and IsSupportingMember = 1
END




ALTER TABLE ManageTeamMemberChangelog
ADD ManagementTeamMemberId INT


ALTER TABLE ManageTeamMemberChangelog
ADD FOREIGN KEY (ManagementTeamMemberId) REFERENCES Management_TeamMember(Id);



GO
/****** Object:  StoredProcedure [dbo].[SP_ManageTeamMemberChangelog_Insert]    Script Date: 3/21/2024 3:18:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



ALTER PROCEDURE [dbo].[SP_ManageTeamMemberChangelog_Insert]
	  @TeamId int,
	  @MemberContactId int,
	  @AssignByContactId int,
	  @AssignTime datetime,
	  --@CancelByContactId int,
	  --@CancelTime datetime,
	  @AssignNote nvarchar(500),
	  --@CancelNote nvarchar(500),
	  @IsSupportingMember bit,
	  @ManagementTeammemberId int

AS
BEGIN  

	Insert Into  [dbo].[ManageTeamMemberChangelog] 
	(
		[TeamId],
		[MemberContactId],
		[AssignByContactId],
		[AssignTime],
		--[CancelByContactId],
		--[CancelTime],
		[AssignNote],
		--[CancelNote],
		[IsSupportingMember],
		[ManagementTeamMemberId]
	)

	Values
	(
		  @TeamId,
		  @MemberContactId,
		  @AssignByContactId,
		  @AssignTime,
		  --@CancelByContactId,
		  --@CancelTime,
		  @AssignNote,
		  --@CancelNote,
		  @IsSupportingMember,
		  @ManagementTeammemberId
	)

	SELECT SCOPE_IDENTITY();

END




GO
/****** Object:  StoredProcedure [dbo].[SP_GetSupportingMemberByTeamIdandContactId]    Script Date: 3/21/2024 10:07:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Rakib	
-- Create date: 20-03-2024
-- Description:	SP_GetSupportingMemberByTeamIdandContactId

-- =============================================
Alter PROCEDURE [dbo].[SP_GetSupportingMemberByTeamIdandContactId]
(
    @TeamId INT,
	@ContactId INT
)
AS
BEGIN
    select * from Management_TeamMember where TeamId = @TeamId and ContactId = @ContactId and IsSupportingMember = 1
END




GO
/****** Object:  StoredProcedure [dbo].[SP_ManageTeamMemberChangelog_UpdateByManagementTeammemberId]    Script Date: 3/22/2024 1:04:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Rakib	
-- Create date: 22-03-2024
-- Description: SP_ManageTeamMemberChangelog_UpdateByManagementTeammemberId

-- =============================================

Alter PROCEDURE [dbo].[SP_ManageTeamMemberChangelog_UpdateByManagementTeammemberId]
	
	  @CancelByContactId int,
	  @CancelTime datetime,
	  @CancelNote nvarchar(500),
	  @ManagementTeammemberId int

AS
BEGIN  
	update dbo.ManageTeamMemberChangelog set CancelByContactId =@CancelByContactId , CancelTime = @CancelTime , CancelNote = @CancelNote 
	where ManagementTeamMemberId = @ManagementTeammemberId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Management_TeamMember_GetListWithDetails]    Script Date: 5/22/2024 4:01:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_Management_TeamMember_GetListWithDetails]
	@TeamId int
AS
BEGIN  

   --IF OBJECT_ID('tempdb..#TempTeamMemberData') IS NOT NULL
    --DROP TABLE #TempTeamMemberData;
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
	  ,TM.IsSupportingMember
	 
--INTO #TempTeamMemberData
FROM [dbo].[Management_TeamMember] AS TM WITH (NOLOCK)
INNER JOIN [dbo].[Management_TeamRole] AS TR WITH (NOLOCK) ON TM.TeamRoleId = TR.Id
INNER JOIN [dbo].[Security_Contact] AS CO WITH (NOLOCK) ON TM.ContactId = CO.Id AND CO.IsDeleted = 0
WHERE TM.[TeamId] = @TeamId
  AND TM.TeamRoleId = 2
  And (TM.ContactId not IN (SELECT ContactId FROM Management_TeamMember WHERE IsSupportingMember = 1 and TeamId != @TeamId))
ORDER BY Id DESC; -- Need To Talk Aminul Vai
END


GO
/****** Object:  StoredProcedure [dbo].[SP_GetTeamMembersWhoSupportAnotherTeamByTeamId]    Script Date: 5/22/2024 5:58:10 PM ******/
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
    SELECT c.Id, c.FirstName, c.LastName,c.DownloadFolderPath,c.IsUserActive 
    FROM [dbo].[Management_TeamMember] mt
    INNER JOIN [dbo].[Security_Contact] c ON mt.ContactId = c.Id
    WHERE mt.ContactId in (select ContactId where IsSupportingMember = 1 and TeamId != @TeamId)
    GROUP BY c.Id, c.FirstName, c.LastName,c.DownloadFolderPath,c.IsUserActive
END




