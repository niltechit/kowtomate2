
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
	
	WHERE TM.[TeamId] = @TeamId and TM.TeamRoleId = 2 order by Id desc -- Need To Talk Aminul Vai

END


GO

ALTER PROCEDURE [dbo].[SP_Security_Contact_Delete](
@ObjectId  varchar(40)        
)
AS
BEGIN  
    Update Security_Contact SET 
	
	IsDeleted=1

	FROM  [dbo].[Security_Contact] WHERE ObjectId = @ObjectId
END

go

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
  where com.ObjectId = @ObjectId AND c.IsDeleted=0  order by c.EmployeeId asc
END

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
  where com.Id=@companyId AND c.IsDeleted=0 order by c.EmployeeId asc

END


GO

ALTER PROCEDURE [dbo].[SP_Security_Contact_GetAll]
AS
BEGIN  
	SELECT *  FROM [dbo].[Security_Contact] where IsDeleted=0
END

GO

ALTER PROCEDURE [dbo].[SP_Security_Contact_GetByEmployeeId]
-- exec SP_Security_Contact_GetByEmployeeId 'P0003'
@employeeId nvarchar(max)
AS
BEGIN  
	SELECT Id FROM [dbo].[Security_Contact] Where EmployeeId= @employeeId
END

go

CREATE PROCEDURE [dbo].[SP_Security_Contact_GetByEmployee]
AS
BEGIN
    SELECT * FROM [dbo].[Security_Contact]  WHERE [EmployeeId] IS NOT NULL and IsDeleted=0 ORDER BY  EmployeeId ASC
END

go

ALTER PROCEDURE [dbo].[SP_Security_Contact_Insert](
             
			@CompanyId int,
			@FirstName nvarchar(100),
			@LastName nvarchar(100),
			@DesignationId int,
            @Email nvarchar(100),
            @Phone varchar(20),
            @ProfileImageUrl varchar(200),
            @Status int,
            @CreatedByContactId int,
            @ObjectId varchar(40),
			@EmployeeId varchar(40)
)
AS
BEGIN  
  INSERT INTO [dbo].[Security_Contact]
           ([CompanyId]
           ,[FirstName]
           ,[LastName]
           ,[DesignationId]
           ,[Email]
           ,[Phone]
           ,[ProfileImageUrl]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[ObjectId]
		   ,[EmployeeId]
		   ,IsDeleted
		   )
     VALUES
           (
		   @CompanyId,
           @FirstName,
           @LastName, 
           @DesignationId, 
           @Email, 
           @Phone, 
           @ProfileImageUrl, 
           @Status, 
           SYSDATETIME(), 
           @CreatedByContactId,
           @ObjectId,
		   @EmployeeId,
		   0
		   )
		   SELECT SCOPE_IDENTITY();
END
go

  update Security_Contact set

  IsDeleted=0

  where IsDeleted is null

  go

  alter table Security_Contact
  alter column IsDeleted bit not null