
ALTER PROCEDURE [dbo].[SP_Security_Contact_GetAllIsSharedFolderEditorContact]
@ConsoleAppId int
AS
BEGIN  
  SET NOCOUNT ON;

 SELECT DISTINCT
		c.*
FROM
    [dbo].[Security_Contact] c 
    INNER JOIN [dbo].[Order_AssignedImageEditor] oie ON oie.AssignContactId = c.Id and oie.IsActive = 1
    INNER JOIN [dbo].[Order_ClientOrderItem] oci ON oci.Id = oie.Order_ImageId
	Inner Join dbo.CompanyGeneralSettings cs on cs.CompanyId = oci.CompanyId AND cs.FtpOrderPlacedAppNo=@ConsoleAppId
	where cs.AllowAutoUploadFromEditorPc = 1 and  oci.Status IN (7,8,11,12) and c.isSharedFolderEnable = 1

END

GO

ALTER PROCEDURE [dbo].[SP_Security_Contact_GetAllIsSharedFolderQcContact]
@ConsoleAppId INT
AS
BEGIN  
  SET NOCOUNT ON;

 
  select cgs.CompanyId into #autoDistributedCompanyIds  from dbo.CompanyGeneralSettings cgs where cgs.AllowAutoUploadFromQcPc =1 AND FtpOrderPlacedAppNo=@ConsoleAppId
  
  select ct.TeamId into #teamIds from dbo.Common_CompanyTeam  ct where CompanyId in (select CompanyId from #autoDistributedCompanyIds)
  
  select mtm.ContactId into #autoUploadContactId from dbo.Management_TeamMember mtm where TeamId in (select TeamId from #teamIds)


  DECLARE @EditorRoleObjectId varchar (100)
  select @EditorRoleObjectId= ObjectId from Security_Role where Name = 'Qc'
  SELECT *
  FROM [dbo].[Security_Contact] where isSharedFolderEnable = 1 and Id in (  select ContactId from Security_User as su inner join Security_UserRole as sur on sur.UserObjectId=su.ObjectId where sur.RoleObjectId= @EditorRoleObjectId) and Id in (select ContactId from #autoUploadContactId)

   drop table  #autoDistributedCompanyIds
   drop table   #teamIds 
   drop table  #autoUploadContactId 
END
