-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	delete by id and isactive and isdeleted field update
-- =============================================
ALTER PROCEDURE [dbo].[SP_Client_Category_DeleteById]
	-- Add the parameters for the stored procedure here
	@Id int,
	@IsActive BIT,
	@IsDeleted bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE Client_Category
    SET
        IsActive = ISNULL(@IsActive, IsActive),
		IsDeleted = ISNULL(@IsDeleted, IsDeleted)

    WHERE
        Id = @Id;
END
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Get Client Categories
-- =============================================
alter PROCEDURE [dbo].[SP_Client_Category_GetAll] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ccs.*, cs.Name as CompanyName, cc.Name as CategoryName,sc.EmployeeId as CreatedByUsername  FROM Client_Category ccs with(nolock) 
	LEFT JOIN Common_Company cs on cs.Id = ccs.ClientCompanyId
	LEFT JOIN Common_Category cc on cc.Id = ccs.CommonCategoryId
	LEFT JOIN Security_User su on su.ObjectId = ccs.CreatedByUsername
	LEFT JOIN Security_Contact sc on sc.Id = su.ContactId
	
	where  ccs.IsDeleted = 0;
END
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Here insert Common Category
-- =============================================
ALTER PROCEDURE [dbo].[SP_Client_Category_Update]
    @Id INT,
    @CommonCategoryId INT,
    @ClientCompanyId INT,
    @TimeInMinutes DECIMAL = NULL,
    @PriceInUSD nvarchar(max) = NULL,
	@IsActive bit = null,
	@IsDeleted bit = null,
	@UpdatedDate datetime ,
	@UpdatedByUsername varchar(50) 
AS
BEGIN
    UPDATE Client_Category
    SET
        CommonCategoryId = ISNULL(@CommonCategoryId, CommonCategoryId),
        ClientCompanyId = ISNULL(@ClientCompanyId, ClientCompanyId),
        TimeInMinutes = ISNULL(@TimeInMinutes, TimeInMinutes),
        PriceInUSD = ISNULL(@PriceInUSD, PriceInUSD),
		IsActive = ISNULL(@IsActive, IsActive),
		IsDeleted = ISNULL(@IsDeleted, IsDeleted),
		UpdatedDate = ISNULL(@UpdatedDate, UpdatedDate),
		UpdatedByUsername = ISNULL(@UpdatedByUsername, UpdatedByUsername)
    WHERE
        Id = @Id;
END;
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	delete by id and isactive and isdeleted field update
-- =============================================
alter PROCEDURE [dbo].[SP_Client_CategoryService_DeleteById]
	-- Add the parameters for the stored procedure here
	@Id int,
	@IsActive BIT,
	@IsDeleted bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE Client_CategoryService
    SET
        IsActive = ISNULL(@IsActive,IsActive),
		IsDeleted =ISNULL(@IsDeleted,IsDeleted)

    WHERE
        Id = @Id;
END
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Get by id common category
-- =============================================
alter PROCEDURE [dbo].[SP_Client_CategoryService_GetById]
	-- Add the parameters for the stored procedure here
	@Id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM Client_CategoryService with(nolock) WHERE Id = @Id
END
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Here update Common Category
-- =============================================
alter PROCEDURE [dbo].[SP_Client_CategoryService_Insert]
    @CommonServiceId INT,
    @ClientCategoryId INT,
    @TimeInMinutes DECIMAL(10,2) = NULL,
    @PriceInUSD nvarchar(max) = NULL,
	@IsActive bit,
	@IsDeleted bit,
	@CreatedByUsername nvarchar(50),
	@CreatedDate datetime
AS
BEGIN

   INSERT INTO Client_CategoryService (CommonServiceId, ClientCategoryId, TimeInMinutes, PriceInUSD,IsActive,IsDeleted,CreatedByUsername,CreatedDate)
    VALUES (@CommonServiceId, @ClientCategoryId, @TimeInMinutes, @PriceInUSD,@IsActive,@IsDeleted,@CreatedByUsername,@CreatedDate);

    -- Get the ID of the inserted record
SELECT SCOPE_IDENTITY();
END;
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Here insert Common Category
-- =============================================
alter PROCEDURE [dbo].[SP_Client_CategoryService_Update]
    @Id INT,
    @ClientCategoryId INT,
    @CommonServiceId INT,
    @TimeInMinutes decimal(10,2) = NULL,
    @PriceInUSD nvarchar(max) = NULL,
	@IsActive bit = null,
	@IsDeleted bit = null,
	@UpdatedByUsername nvarchar(50),
	@UpdatedDate datetime = null
AS
BEGIN
    UPDATE Client_CategoryService
    SET
        --ClientCategoryId = ISNULL(@ClientCategoryId, ClientCategoryId),
        --CommonServiceId = ISNULL(@CommonServiceId, CommonServiceId),
        TimeInMinutes = ISNULL(@TimeInMinutes, TimeInMinutes),
        PriceInUSD = ISNULL(@PriceInUSD, PriceInUSD),
		IsActive = ISNULL(@IsActive, IsActive),
		IsDeleted = ISNULL(@IsDeleted, IsDeleted),
		UpdatedByUsername = ISNULL(@UpdatedByUsername, UpdatedByUsername),
		UpdatedDate = ISNULL(@UpdatedDate, UpdatedDate)
    WHERE
        Id = @Id;
END;
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Get Client Categories
-- =============================================
alter PROCEDURE [dbo].[SP_Common_Category_GetAll] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM Common_Category with(nolock) where IsDeleted = 0;
END
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Here insert Common Category
-- =============================================
alter PROCEDURE [dbo].[SP_Common_Category_Insert]
    @Name NVARCHAR(MAX),
    @TimeInMinutes DECIMAL(10,2),
    @PriceInUSD nvarchar(max),
    @IsActive BIT,
    @CreatedDate DATETIME,
    @CreatedByUsername NVARCHAR(MAX),
    @UpdatedDate DATETIME = NULL,
    @UpdatedByUsername NVARCHAR(MAX) = NULL
AS
BEGIN

    INSERT INTO Common_Category(Name, TimeInMinutes, PriceInUSD, IsActive, CreatedDate, CreatedByUsername, UpdatedDate, UpdatedByUsername)
    VALUES (@Name, @TimeInMinutes, @PriceInUSD, @IsActive, @CreatedDate, @CreatedByUsername, @UpdatedDate, @UpdatedByUsername);

    -- Get the ID of the inserted record
    SELECT SCOPE_IDENTITY();

    -- Return the inserted ID
END;
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Here update Common Category
-- =============================================
alter PROCEDURE [dbo].[SP_Common_Category_Update]
    @Id INT,
    @Name NVARCHAR(MAX),
    @TimeInMinutes DECIMAL,
    @PriceInUSD nvarchar(max),
    @IsActive BIT,
    @UpdatedDate DATETIME,
    @UpdatedByUsername NVARCHAR(MAX),
	@IsDeleted BIT
AS
BEGIN
    UPDATE Common_Category
    SET
        Name = ISNULL(@Name, Name),
        TimeInMinutes = ISNULL(@TimeInMinutes, TimeInMinutes),
        PriceInUSD = ISNULL(@PriceInUSD, PriceInUSD),
        IsActive = ISNULL(@IsActive, IsActive),
        UpdatedDate = ISNULL(@UpdatedDate, UpdatedDate),
        UpdatedByUsername = ISNULL(@UpdatedByUsername, UpdatedByUsername),
		IsDeleted = ISNULL(@IsDeleted,IsDeleted)
    WHERE
        Id = @Id;
END;
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Here update Common Category
-- =============================================
alter PROCEDURE [dbo].[SP_Common_CategoryService_Insert]
    @CommonCategoryId INT,
    @CommonServiceId INT,
    @TimeInMinutes decimal(10,2) = NULL,
    @PriceInUSD nvarchar(max) = NULL,
	@IsActive bit,
	@IsDeleted bit
AS
BEGIN

   INSERT INTO Common_CategoryService (CommonCategoryId, CommonServiceId, TimeInMinutes, PriceInUSD,IsActive,IsDeleted)
    VALUES (@CommonCategoryId, @CommonServiceId, @TimeInMinutes, @PriceInUSD,@IsActive,@IsDeleted);

    -- Get the ID of the inserted record
    SELECT SCOPE_IDENTITY();
END;
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Here insert Common Category
-- =============================================
alter PROCEDURE [dbo].[SP_Common_CategoryService_Update]
    @Id INT,
    @CommonCategoryId INT,
    @CommonServiceId INT,
    @TimeInMinutes DECIMAL(10,2) = NULL,
    @PriceInUSD nvarchar(max) = NULL,
	@IsActive bit = null,
	@IsDeleted bit = null
AS
BEGIN
    UPDATE Common_CategoryService
    SET
        --CommonCategoryId = ISNULL(@CommonCategoryId, CommonCategoryId),
        --CommonServiceId = ISNULL(@CommonServiceId, CommonServiceId),
        TimeInMinutes = ISNULL(@TimeInMinutes, TimeInMinutes),
        PriceInUSD = ISNULL(@PriceInUSD, PriceInUSD),
		IsActive = ISNULL(@IsActive, IsActive),
		IsDeleted = ISNULL(@IsDeleted, IsDeleted)
    WHERE
        Id = @Id;
END;
GO

alter PROCEDURE [dbo].[SP_Common_Service_GetAll] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
		* 
	FROM 
		Common_Service with(nolock) 
	WHERE 
		 IsDeleted = 0;
END
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Here insert Common Category
-- =============================================
alter PROCEDURE [dbo].[SP_Common_Service_Insert]
    @Name NVARCHAR(MAX),
    @TimeInMinutes DECIMAL(10,2),
    @PriceInUSD nvarchar(max),
    @IsActive BIT,
    @CreatedDate DATETIME,
    @CreatedByUsername NVARCHAR(MAX),
    @UpdatedDate DATETIME = NULL,
    @UpdatedByUsername NVARCHAR(MAX) = NULL
AS
BEGIN
    DECLARE @Id INT;

    INSERT INTO Common_Service(Name, TimeInMinutes, PriceInUSD, IsActive, CreatedDate, CreatedByUsername, UpdatedDate, UpdatedByUsername)
    VALUES (@Name, @TimeInMinutes, @PriceInUSD, @IsActive, @CreatedDate, @CreatedByUsername, @UpdatedDate, @UpdatedByUsername);

    -- Get the ID of the inserted record
    SET @Id = SCOPE_IDENTITY();

    -- Return the inserted ID
    RETURN @Id;
END;
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Here update Common Category
-- =============================================
alter PROCEDURE [dbo].[SP_Common_Service_Update]
    @Id INT,
    @Name NVARCHAR(MAX),
    @TimeInMinutes DECIMAL(10,2),
    @PriceInUSD NVARCHAR(MAX),
    @IsActive BIT,
    @UpdatedDate DATETIME,
    @UpdatedByUsername NVARCHAR(MAX),
	@IsDeleted bit 
AS
BEGIN
    UPDATE Common_Service
    SET
        Name = ISNULL(@Name, Name),
        TimeInMinutes = ISNULL(@TimeInMinutes, TimeInMinutes),
        PriceInUSD = ISNULL(@PriceInUSD, PriceInUSD),
        IsActive = ISNULL(@IsActive, IsActive),
        UpdatedDate = ISNULL(@UpdatedDate, UpdatedDate),
        UpdatedByUsername = ISNULL(@UpdatedByUsername, UpdatedByUsername),
		IsDeleted = ISNULL(@IsDeleted, IsDeleted)
    WHERE
        Id = @Id;
END;
GO

-- =============================================
-- Author:	Rakib	
-- Create date: 20-03-2024
-- Description:	SP_GetTeamMembersWhoSupportAnotherTeamByTeamId
-- =============================================
alter PROCEDURE [dbo].[SP_GetTeamMembersWhoSupportAnotherTeamByTeamId]
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

alter PROCEDURE [dbo].[SP_HR_Designation_GetByObjectId]
@ObjectId varchar(40)
AS
BEGIN  

	SELECT  *
	FROM [dbo].[HR_Designation] WHERE [ObjectId] = @ObjectId

END
GO

-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

alter PROCEDURE [dbo].[SP_HR_Designation_Insert](
            @Name  nvarchar(100),
            @Status int,
			@ObjectId varchar(40),
            @CreatedByContactId int,
			@DayOffMonday bit,
			@DayOffTuesday bit,
			@DayOffWednesday bit,
			@DayOffThursday bit,
			@DayOffFriday bit,
			@DayOffSaturday bit,
			@DayOffSunday bit
)
AS
BEGIN  

    Insert Into  [dbo].[HR_Designation] 
	(
		Name,
		Status,
		CreatedDate,
		CreatedByContactId,
		ObjectId,
		DayOffMonday,
		DayOffTuesday,
		DayOffWednesday,
		DayOffThursday,
		DayOffFriday,
		DayOffSaturday,
		DayOffSunday
	)

	Values
	(
	    @Name,
		@Status,
		SYSDATETIME(),
		@CreatedByContactId, 
		@ObjectId,
		@DayOffMonday,  -- Set boolean values for each day
		@DayOffTuesday,
		@DayOffWednesday,
		@DayOffThursday,
		@DayOffFriday,
		@DayOffSaturday,
		@DayOffSunday
	)

	SELECT SCOPE_IDENTITY();
  
END
GO


-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

alter PROCEDURE [dbo].[SP_HR_Designation_Update](
            @Id  int,
            @Name  nvarchar(100),
            @Status int,
            @UpdatedByContactId int,
			@DayOffMonday bit,
			@DayOffTuesday bit,
			@DayOffWednesday bit,
			@DayOffThursday bit,
			@DayOffFriday bit,
			@DayOffSaturday bit,
			@DayOffSunday bit
)
AS
BEGIN  
    UPDATE [dbo].[HR_Designation]
      SET 
	  [Name] = @Name,
      [Status] = @Status,    
      [UpdatedDate] = SYSDATETIME(),
      [UpdatedByContactId] = @UpdatedByContactId,
	  [DayOffMonday] = @DayOffMonday,
	  [DayOffTuesday] = @DayOffTuesday,
	  [DayOffWednesday] = @DayOffWednesday,
	  [DayOffThursday] = @DayOffThursday,
	  [DayOffFriday] = @DayOffFriday,
	  [DayOffSaturday] = @DayOffSaturday,
	  [DayOffSunday] = @DayOffSunday

      WHERE Id = @Id
END
GO

alter PROCEDURE [dbo].[SP_Management_TeamMember_GetListWithDetails]
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

--SELECT * FROM #TempTeamMemberData where (ContactId not IN (SELECT ContactId FROM Management_TeamMember WHERE IsSupportingMember = 1 and TeamId != @TeamId))


END
GO

-- =======================
-- Author:		Aminul
-- Create date: 22-08-2022
-- Description:	<Description,,>
-- Reference: 
-- EXEC [dbo].[SP_Order_ClientOrder_GetListByFilter] ''
-- =======================
alter PROCEDURE [dbo].[SP_Order_ClientOrder_GetListByFilter]
	@Where NVARCHAR(3000)='',
	@IsCalculateTotal BIT='true',
	@Skip INT = 0,
	@Top INT = 20,
	@SortColumn NVARCHAR(50) = 'o.[OrderPlaceDate]',
	@SortDirection NVARCHAR(4)='DESC'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE 
	@TotalCount INT=0,
	@TotalImageCount DECIMAL(16,0) = 0,
	@SQL NVARCHAR(MAX),
	@FinalSQL NVARCHAR(MAX),		
	@OutPut1 NVARCHAR(max)

	--get totals if page search
	IF(@IsCalculateTotal='true')
	BEGIN

			SET @SQL = N'
			SELECT 
			@TotalCount =COUNT(*),
			@TotalImageCount = ISNULL(SUM(o.[NumberOfImage]),0)
			FROM [dbo].[Order_ClientOrder] o WITH(NOLOCK) 
	left JOIN [dbo].Security_Contact assignby WITH(NOLOCK) ON assignby.Id=o.[AssignedByOpsContactId] 
	Left JOIN [dbo].Common_Company c WITH(NOLOCK) ON c.Id = o.CompanyId
	left JOIN dbo.Management_Team T WITH(NOLOCK) ON T.Id=o.AssignedTeamId '
			+@Where
		
	SET @OutPut1 = N'@TotalCount INT OUTPUT,@TotalImageCount DECIMAL(16,0) OUTPUT ';
	EXEC sp_executesql @SQL, @OutPut1, @TotalCount =@TotalCount OUTPUT,@TotalImageCount = @TotalImageCount OUTPUT;
	END	
	--select possible columns
	EXECUTE 
	('SELECT  '	
	+@TotalCount+' TotalCount, '
	+@TotalImageCount +' TotalImageCount 
	   ,o.[Id]
      ,o.[CompanyId]
	  ,c.[ObjectId] CompanyObjectId
	  ,c.[Name] CompanyName
      ,o.[FileServerId]
      ,o.[OrderNumber]
      ,o.[OrderPlaceDate]
      ,o.[ExpectedDeliveryDate]
      ,o.[ProcessingCompletedDate]
      ,o.[InternalQcRequestDate]
      ,o.[InternalQcCompleteDate]
      ,o.[ClientQcRequestDate]
      ,o.[DeliveredDate]
      ,o.[InvoiceDate]
      ,o.[ExternalOrderStatus]
      ,o.[InternalOrderStatus]
      ,o.[IsDeleted]
      ,o.[CreatedDate]
      ,o.[CreatedByContactId]
      ,o.[UpdatedDate]
      ,o.[UpdatedByContactId]
      ,o.[ObjectId]
	  ,o.[NumberOfImage]
	  ,o.[ExternalOrderStatus]
	  ,o.AllowExtraOutputFileUpload
	  ,dateadd(d, datediff(d,0, o.[OrderPlaceDate]), 0) OrderPlaceDateOnly
	  ,assignby.[FirstName] ContactName
	  ,T.[Name] TeamName
	  ,o.[AssignedDateToTeam] TeamAssignedDate
	  ,o.[ArrivalTime]
	  ,o.[DeliveryDeadlineInMinute]
	  ,c.[DeliveryDeadlineInMinute] CompanyDeliveryDeadlineInMinute
	  ,cf.Username SourceFtpUsername
	  ,o.BatchPath
	  ,o.CategorySetStatus
	FROM [dbo].[Order_ClientOrder] o WITH(NOLOCK)
	left JOIN [dbo].Security_Contact assignby WITH(NOLOCK) ON assignby.Id=o.[AssignedByOpsContactId] 
	Left JOIN [dbo].Common_Company c WITH(NOLOCK) ON c.Id = o.CompanyId
	Left JOIN dbo.Client_ClientOrderFtp cf WITH(NOLOCK) ON cf.Id = o.SourceServerId
	left JOIN dbo.Management_Team T WITH(NOLOCK) ON T.Id=o.AssignedTeamId '
	+@Where
	
	+' ORDER BY '+@SortColumn +' '+ @SortDirection+' '
	+'OFFSET '+ @Skip+' ROWS '
	+'FETCH NEXT '+@Top+' ROWS ONLY' 
	)
END
GO

alter PROCEDURE [dbo].[SP_Order_ClientOrderAssignedItem_GetByOrderId]
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
	  ,c_cat.Name as CategoryName
	    ,orderitem.CategorySetStatus
	  ,orderitem.CategoryApprovedByContactId
	  ,orderitem.CategoryPrice
	  ,orderitem.CategorySetByContactId
	  ,orderitem.CategorySetDate
	  ,orderitem.ClientCategoryId
	from Order_ClientOrderItem as orderitem
	inner join Order_AssignedImageEditor as assignorder WITH(NOLOCK) on orderitem.Id = assignorder.Order_ImageId and assignorder.IsActive = 1 
	inner join dbo.Security_Contact as contact  WITH(NOLOCK) on contact.Id=assignorder.AssignContactId
	left join dbo.Management_Team as mt  WITH(NOLOCK) on mt.Id = orderitem.TeamId
	left join dbo.Client_Category as cc With(Nolock) on cc.Id = orderitem.ClientCategoryId
    left join dbo.Common_Category as c_cat with(NOLock) on c_cat.Id = cc.CommonCategoryId
	where orderitem.ClientOrderId=@OrderId and assignorder.AssignContactId = @ContactId and orderitem.FileGroup <> 4 --4 means ColorRef
end
GO

alter PROCEDURE [dbo].[SP_Order_ClientOrderAssignedItem_GetByOrderIdContactIdTeamId]
(
	@OrderId int,
	@ContactId int,
	@TeamId int
)
as
begin
	SELECT orderitem.[Id]
      ,orderitem.[CompanyId]
      ,[ClientOrderId]
      ,[FileName]
      ,[ExteranlFileInputPath]
      ,[ExternalFileOutputPath]
	  ,[InternalFileInputPath]
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
	  ,orderitem.FileByteString
	  ,orderitem.ProductionFileByteString
	  ,orderitem.ProductionDoneFilePath
	  ,orderitem.ExpectedDeliveryDate
	  ,orderitem.FileGroup
	  ,assignorder.AssignDate as OrderAssignDate
	  ,contact.FirstName as EditorFirstName
	  ,contact.LastName as EditorLastName
	  ,contact.EmployeeId
	  ,assignorder.AssignContactId
	  ,mt.Name as TeamName
	  ,c_cat.Name as CategoryName
	    ,orderitem.CategorySetStatus
	  ,orderitem.CategoryApprovedByContactId
	  ,orderitem.CategoryPrice
	  ,orderitem.CategorySetByContactId
	  ,orderitem.CategorySetDate
	  ,orderitem.ClientCategoryId
	from Order_ClientOrderItem as orderitem 
	left join Order_AssignedImageEditor as assignorder With(NoLock) on orderitem.Id = assignorder.Order_ImageId and assignorder.IsActive = 1
	left join dbo.Security_Contact as contact With(NoLock) on contact.Id=assignorder.AssignContactId
	left join dbo.Management_Team as mt With(NoLock) on mt.Id = orderitem.TeamId
	left join dbo.Client_Category as cc With(Nolock) on cc.Id = orderitem.ClientCategoryId
    left join dbo.Common_Category as c_cat with(NOLock) on c_cat.Id = cc.CommonCategoryId

	where orderitem.ClientOrderId=@OrderId  and orderitem.TeamId = @TeamId and orderItem.FileGroup <> 4 --4 means ColorRef
end
GO

-- =============================================
-- Author:	Md Zakir Hossain	
-- Create date: 09 Sept 2022
-- Description:	Get All Order Item by Order Id
-- =============================================

ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetAllByOrderId]
	@OrderId int
AS
BEGIN  
	
	
	SELECT orderitem.[Id]
      ,orderitem.[CompanyId]
      ,[ClientOrderId]
      ,[FileName]
      ,[ExteranlFileInputPath]
      ,[ExternalFileOutputPath]
	  ,[InternalFileInputPath]
      ,[InternalFileOutputPath]
	  ,[PartialPath]
      ,[UnitPrice]
      ,orderitem.[IsDeleted]
      ,orderitem.[CreatedDate]
      ,orderitem.[CreatedByContactId]
      ,orderitem.[UpdatedDate]
      ,orderitem.[UpdatedByContactId]
      ,orderitem.[ObjectId]
	  ,orderitem.TeamId
	  ,orderitem.ProductionDoneFilePath
	  ,orderitem.ProductionDoneFilePath
	  ,orderitem.FileNameWithoutExtension
	  ,orderitem.ExpectedDeliveryDate
      ,[FileSize]
	  ,orderitem.FileByteString
	  ,orderitem.Status as Status
	  ,orderitem.ExternalStatus
	  ,orderitem.FileGroup
	  ,orderitem.ExpectedDeliveryDate
	  ,assignorder.AssignDate as OrderAssignDate
	  ,contact.FirstName as EditorFirstName
	  ,contact.LastName as EditorLastName
	  ,contact.EmployeeId
	  ,mt.Name as TeamName,
	  orderitem.ArrivalTime,
	  c_cat.Name as CategoryName
	   ,orderitem.CategorySetStatus
	  ,orderitem.CategoryApprovedByContactId
	  ,orderitem.CategoryPrice
	  ,orderitem.CategorySetByContactId
	  ,orderitem.CategorySetDate
	  ,orderitem.ClientCategoryId
  FROM [dbo].[Order_ClientOrderItem] as orderitem 
  left join dbo.Order_AssignedImageEditor as assignorder With(Nolock) on orderitem.Id = assignorder.Order_ImageId and assignorder.IsActive = 1
  left join dbo.Security_Contact as contact With(Nolock) on contact.Id=assignorder.AssignContactId
  left join dbo.Management_Team as mt With(Nolock) on mt.Id = orderitem.TeamId
  left join dbo.Client_Category as cc With(Nolock) on cc.Id = orderitem.ClientCategoryId
  left join dbo.Common_Category as c_cat with(NOLock) on c_cat.Id = cc.CommonCategoryId
  
   where ClientOrderId=@OrderId and (orderitem.IsDeleted=0 or orderItem.IsDeleted is null)  and orderItem.FileGroup <> 4 --4 means ColorRef


END
GO

-- =============================================
-- Author:	Md Rakib Hossain	
-- Create date: 26 March 2024
-- Description:	Get All Order Item by Order Id For Client
-- =============================================

ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetAllByOrderIdForClient]
	@OrderId int
AS
BEGIN  
	
	
	SELECT orderitem.[Id]
      ,orderitem.[CompanyId]
      ,[ClientOrderId]
      ,[FileName]
      ,[ExteranlFileInputPath]
      ,[ExternalFileOutputPath]
	  ,[InternalFileInputPath]
      ,[InternalFileOutputPath]
	  ,[PartialPath]
      ,[UnitPrice]
      ,orderitem.[IsDeleted]
      ,orderitem.[CreatedDate]
      ,orderitem.[CreatedByContactId]
      ,orderitem.[UpdatedDate]
      ,orderitem.[UpdatedByContactId]
      ,orderitem.[ObjectId]
	  ,orderitem.TeamId
	  ,orderitem.ProductionDoneFilePath
	  ,orderitem.ProductionDoneFilePath
	  ,orderitem.FileNameWithoutExtension
	  ,orderitem.ExpectedDeliveryDate
      ,[FileSize]
	  ,orderitem.FileByteString
	  ,orderitem.Status as Status
	  ,orderitem.ExternalStatus
	  ,orderitem.FileGroup
	  ,orderitem.ExpectedDeliveryDate
	  ,assignorder.AssignDate as OrderAssignDate
	  ,contact.FirstName as EditorFirstName
	  ,contact.LastName as EditorLastName
	  ,contact.EmployeeId
	  ,mt.Name as TeamName,
	  orderitem.ArrivalTime,
	  c_cat.Name as CategoryName
	  ,orderitem.CategorySetStatus
	  ,orderitem.CategoryApprovedByContactId
	  ,orderitem.CategoryPrice
	  ,orderitem.CategorySetByContactId
	  ,orderitem.CategorySetDate
	  ,orderitem.ClientCategoryId
  FROM [dbo].[Order_ClientOrderItem] as orderitem 
  left join dbo.Order_AssignedImageEditor as assignorder With(Nolock) on orderitem.Id = assignorder.Order_ImageId and assignorder.IsActive = 1
  left join dbo.Security_Contact as contact With(Nolock) on contact.Id=assignorder.AssignContactId
  left join dbo.Management_Team as mt With(Nolock) on mt.Id = orderitem.TeamId
  left join dbo.Client_Category as cc With(Nolock) on cc.Id = orderitem.ClientCategoryId
  left join dbo.Common_Category as c_cat with(NOLock) on c_cat.Id = cc.CommonCategoryId
  
   where ClientOrderId=@OrderId and (orderitem.IsDeleted=0 or orderItem.IsDeleted is null)  and orderItem.FileGroup = 1 --1 means Work


END
GO

ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetEqualAndGreaterItemByStatus]
(
	@OrderId int,
	@Status tinyint
)
as
begin
	select orderitem.[Id]
      ,orderitem.[CompanyId]
      ,[ClientOrderId]
      ,[FileName]
      ,[ExteranlFileInputPath]
      ,[ExternalFileOutputPath]
	  ,[InternalFileInputPath]
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
	  ,orderitem.FileByteString
	  ,orderitem.ProductionFileByteString
	  ,orderitem.ProductionDoneFilePath
	  ,orderitem.FileGroup
	  ,orderitem.ExpectedDeliveryDate
	  ,assignorder.AssignDate as OrderAssignDate
	  ,contact.FirstName as EditorFirstName
	  ,contact.LastName as EditorLastName
	  ,contact.EmployeeId
	  ,assignorder.AssignContactId
	   ,mt.Name as TeamName
	   ,orderitem.ArrivalTime
	   ,c_cat.Name as CategoryName
	     ,orderitem.CategorySetStatus
	  ,orderitem.CategoryApprovedByContactId
	  ,orderitem.CategoryPrice
	  ,orderitem.CategorySetByContactId
	  ,orderitem.CategorySetDate
	  ,orderitem.ClientCategoryId
	from dbo.Order_ClientOrderItem as orderitem 
	left join Order_AssignedImageEditor as assignorder WITH(NOLOCK) on orderitem.Id = assignorder.Order_ImageId and assignorder.IsActive = 1
	left join dbo.Security_Contact as contact WITH(NOLOCK) on contact.Id=assignorder.AssignContactId
	left join dbo.Management_Team as mt WITH(NOLOCK) on mt.Id = orderitem.TeamId
	left join dbo.Client_Category as cc With(Nolock) on cc.Id = orderitem.ClientCategoryId
    left join dbo.Common_Category as c_cat with(NOLock) on c_cat.Id = cc.CommonCategoryId
	 where ClientOrderId = @OrderId and orderitem.Status >= @Status and orderItem.FileGroup <> 4 --4 means ColorRef
end
GO

-- =============================================
ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetItemByCompanyIdAndFullFileNameAndFilePath]
	(
		@ClientOrderId int,
		@FileName nvarchar(200),
		@PartialPath nvarchar(250),
		@CompanyId int
	)
AS
BEGIN
   
   SET NOCOUNT ON;

   select * INTO #TempItems1 From 
   Order_ClientOrderItem 
   where ClientOrderId=@ClientOrderId 
   and FileName=@FileName 
   and PartialPath=@PartialPath 
   and CompanyId=@CompanyId

   if ( (select COUNT(*) FROM #TempItems1) = 0)
   BEGIN
        DECLARE @PartialNewPath varchar(1000), @OrderNumber varchar(50)
	    Select @OrderNumber OrderNumber FROM  [dbo].[Order_ClientOrder] WITH(NOLOCK) WHERE Id = @ClientOrderId 
        
		SET @OrderNumber = '/' + @OrderNumber + ''

		SET @PartialNewPath = Replace(@PartialPath, @OrderNumber, '')
		
		select *INTO #TempItems2 From 
		Order_ClientOrderItem WITH(NOLOCK)
		where ClientOrderId=@ClientOrderId 
		and FileName=@FileName 
		and PartialPath LIKE '%'+  @PartialNewPath 
		and CompanyId=@CompanyId
		-- Added Zakir --- 
		and [FileGroup]= 1  -- 1 Means workable file
		 if ( (select COUNT(*) FROM #TempItems2) > 1)
		 BEGIN
		     select * FROM #TempItems2 
		 END
		 ELSE
		 BEGIN
		     return
		 END
   END
   ELSE
   BEGIN
       SELECT *FROM #TempItems1
   END
END
GO

ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItem_insert]
(
	
	@CompanyId int,
	@FileName nvarchar(max),
	@ClientOrderId int,

	@PartialPath nvarchar(250),
	@Status int,
	@IsDeleted bit,
	@CreatedDate datetime,
	@UpdatedDate datetime,
	@ObjectId nvarchar(max),
	@FileSize bigint,
	@TeamId int,
	@ExternalStatus int,
	@FileByteString nvarchar(max),
	@InternalFileOutputPath nvarchar(max),
	@InternalFileInputPath nvarchar(max),
	@ExternalFileOutputPath nvarchar(max),
	@FileNameWithoutExtension nvarchar(200),
	@FileGroup int,
	@IsExtraOutPutFile bit,
	@ArrivalTime datetime,
	@CategoryId int,
	@CategorySetByContactId int,
	@CategorySetDate datetime,
	@CategoryPrice nvarchar(255),
	@CategorySetStatus tinyint,
	@CategoryApprovedByContactId int
)
as
begin
	insert into 
	Order_ClientOrderItem([FileName],ClientOrderId,[Status],IsDeleted, CreatedDate,UpdatedDate,ObjectId,FileSize,ExternalStatus,FileByteString,InternalFileOutputPath,InternalFileInputPath,ExternalFileOutputPath,
	CompanyId,PartialPath,FileNameWithoutExtension,[FileGroup],IsExtraOutPutFile,ArrivalTime,
	ClientCategoryId,CategorySetByContactId,CategorySetDate,CategoryPrice,CategorySetStatus,CategoryApprovedByContactId)
					  
	values(@FileName,@ClientOrderId,@Status,@IsDeleted, @CreatedDate,@UpdatedDate,@ObjectId,
	@FileSize,@ExternalStatus,@FileByteString,@InternalFileOutputPath,@InternalFileInputPath,
	@ExternalFileOutputPath,@CompanyId,@PartialPath,@FileNameWithoutExtension,@FileGroup,
	@IsExtraOutPutFile,@ArrivalTime,@CategoryId,@CategorySetByContactId,@CategorySetDate,@CategoryPrice,
	@CategorySetStatus,@CategoryApprovedByContactId)

    SELECT SCOPE_IDENTITY();
end



SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_Report_DynamicReportInfo_Insert](
	@Name  nvarchar(100),
	@Description  nvarchar(500),
	@SqlType  tinyint,
	@SqlScript  nvarchar(max),
	@AllowCompanyFilter bit,
    @AllowStartDateFilter bit,
    @AllowEndDateFilter bit,
    @AllowDateOnlyFilter bit,
    @AllowFiltering bit,
    @AllowPaging bit,
    @AllowSorting bit,
    @AllowHtmlPreview bit,
    @DefaultSortColumn varchar(255),
    @DefaultSortOrder varchar(10),
    @AllowVirtualization bit,
    @PageSize int,
	@PermissionObjectId varchar(40),
	@Status tinyint,
	@ObjectId varchar(40),
	@CreatedByContactId int,
	/*
	@AllowDetailReport bit,	
	@SqlTypeForDetailReport  tinyint,
	@SqlScriptForDetailReport  nvarchar(max),
	@FilterByForDetailReport  nvarchar(50),
	*/
	@ReportCode  nvarchar(50),
	@ReportType tinyint,
	@WhereClause  nvarchar(max)
)
AS
BEGIN  

    Insert Into  [dbo].[Report_DynamicReportInfo] 
	(
		[Name],
		[Description],
		[SqlType],
		[SqlScript],
		AllowCompanyFilter,
		AllowStartDateFilter,
		AllowEndDateFilter,
		AllowDateOnlyFilter,
		AllowFiltering,
		AllowPaging,
		AllowSorting,
		AllowHtmlPreview,
		DefaultSortColumn,
		DefaultSortOrder,
		AllowVirtualization,
		PageSize,
		PermissionObjectId,
		[Status],
		CreatedDate,
		CreatedByContactId,
		ObjectId,
		/*
		AllowDetailReport,		
		SqlTypeForDetailReport,
		SqlScriptForDetailReport,
		FilterByForDetailReport,
		*/
		ReportCode,
		ReportType,
		WhereClause
	)
	Values
	(
	    @Name,
		@Description,
		@SqlType,
		@SqlScript,
		@AllowCompanyFilter,
		@AllowStartDateFilter,
		@AllowEndDateFilter,
		@AllowDateOnlyFilter,
		@AllowFiltering,
		@AllowPaging,
		@AllowSorting,
		@AllowHtmlPreview,
		@DefaultSortColumn,
		@DefaultSortOrder,
		@AllowVirtualization,
		@PageSize,
		@PermissionObjectId,
		@Status,
		GETDATE(),
		@CreatedByContactId, 
		@ObjectId,
		/*
		@AllowDetailReport,		
		@SqlTypeForDetailReport,
		@SqlScriptForDetailReport,
		@FilterByForDetailReport,
		*/
		@ReportCode,
		@ReportType,
		@WhereClause
	)

	SELECT SCOPE_IDENTITY();
END
GO

ALTER PROCEDURE [dbo].[SP_Report_DynamicReportInfo_Update](
    @Id  int,
    @Name  nvarchar(100),
	@Description  nvarchar(500),
	@SqlType  tinyint,
	@SqlScript  nvarchar(max),
	@AllowCompanyFilter bit,
    @AllowStartDateFilter bit,
    @AllowEndDateFilter bit,
    @AllowDateOnlyFilter bit,
    @AllowFiltering bit,
    @AllowPaging bit,
    @AllowSorting bit,
    @AllowHtmlPreview bit,
    @DefaultSortColumn varchar(255),
    @DefaultSortOrder varchar(10),
    @AllowVirtualization bit,
    @PageSize int,
	@PermissionObjectId varchar(40),
    @Status int,
    @UpdatedByContactId int,
	/*
	@AllowDetailReport bit,	
	@SqlTypeForDetailReport  tinyint,
	@SqlScriptForDetailReport  nvarchar(max),
	@FilterByForDetailReport  nvarchar(50),
	*/
	@ReportCode  nvarchar(50),
	@ReportType tinyint,
	@WhereClause  nvarchar(max)
)
AS
BEGIN  
    UPDATE [dbo].[Report_DynamicReportInfo]
    SET 
	    [Name] = @Name,
	    [Description] = @Description,
	    [SqlType] = @SqlType,
	    [SqlScript] = @SqlScript,
        [Status] = @Status,    
        [UpdatedDate] = GETDATE(),
        [UpdatedByContactId] = @UpdatedByContactId,
	    [AllowCompanyFilter] = @AllowCompanyFilter,
        [AllowStartDateFilter] = @AllowStartDateFilter,
        [AllowEndDateFilter] =  @AllowEndDateFilter,
        [AllowDateOnlyFilter] = @AllowDateOnlyFilter,
        [AllowFiltering] = @AllowFiltering,
        [AllowPaging] = @AllowPaging,
        [AllowSorting] = @AllowSorting,
        [AllowHtmlPreview] = @AllowHtmlPreview,
        [DefaultSortColumn] = @DefaultSortColumn,
        [DefaultSortOrder] = @DefaultSortOrder,
        [AllowVirtualization] = @AllowVirtualization,
        [PageSize] = @PageSize,
		PermissionObjectId = @PermissionObjectId,
		/*
		AllowDetailReport=@AllowDetailReport,		
		SqlTypeForDetailReport=@SqlTypeForDetailReport,
		SqlScriptForDetailReport=@SqlScriptForDetailReport,
		FilterByForDetailReport=@FilterByForDetailReport,
		*/
		ReportCode=@ReportCode,
		ReportType=@ReportType,
		WhereClause=@WhereClause
    WHERE Id = @Id;
END
GO

ALTER PROCEDURE [dbo].[SP_Report_TableColumn_Insert](
    @DynamicReportInfoId INT,
    @DisplayName NVARCHAR(255),
    @FieldName NVARCHAR(255),
    @FieldWithPrefix NVARCHAR(1000),
    @IsVisible BIT,
    @Filterable BIT,
    @Sortable BIT,
    @TextAlign SMALLINT,
    @DisplayOrder INT,
    @Width NVARCHAR(10),
    @TextColor NVARCHAR(50),
    @FieldType SMALLINT,
	@DispalyFormat varchar(100),
    @BackgroundColor VARCHAR(200),
	@BackgroundColorRules varchar(200),
    @ShowFooterTotal BIT,
    @FooterTotalLabel VARCHAR(100),
    @ShowFooterAverage BIT,
    @FooterAverageLabel VARCHAR(100),
    @ApplyInFilter1 BIT,
    @ApplyInFilter2 BIT,
    @ApplyInFilter3 BIT,
	@Groupable BIT,
    @IsDefaultGroup BIT,
	@ShowGroupTotal BIT,
	@JoinInfoId INT,
	@JoinInfo2Id INT,
	@JoinInfo3Id INT,
	@JoinInfo4Id INT,
	@JoinInfo5Id INT,
	@SortingType SMALLINT,
	@CreatedDate DATETIME,
    @CreatedByContactId INT
)
AS
BEGIN
    SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM [dbo].Report_TableColumn WHERE DynamicReportInfoId = @DynamicReportInfoId AND DisplayOrder = @DisplayOrder)
    BEGIN
	   UPDATE [dbo].Report_TableColumn SET DisplayOrder = DisplayOrder + 1  WHERE DynamicReportInfoId = @DynamicReportInfoId AND DisplayOrder >= @DisplayOrder
	END
    
    INSERT INTO [dbo].Report_TableColumn (DynamicReportInfoId, DisplayName, FieldName, FieldWithPrefix, 
	IsVisible, Filterable, Sortable, TextAlign, DisplayOrder, Width, TextColor, FieldType, DispalyFormat,
	CreatedDate, CreatedByContactId, BackgroundColor, BackgroundColorRules, ShowFooterTotal, FooterTotalLabel, 
	ShowFooterAverage, FooterAverageLabel, ApplyInFilter1, ApplyInFilter2, ApplyInFilter3, Groupable,IsDefaultGroup, ShowGroupTotal,
	JoinInfoId,JoinInfo2Id,JoinInfo3Id,JoinInfo4Id,JoinInfo5Id, SortingType)
    VALUES (@DynamicReportInfoId, @DisplayName, @FieldName, @FieldWithPrefix, 
	@IsVisible, @Filterable, @Sortable, @TextAlign, @DisplayOrder, @Width, @TextColor, 
	@FieldType,@DispalyFormat, @CreatedDate, @CreatedByContactId, @BackgroundColor, @BackgroundColorRules, @ShowFooterTotal, 
	@FooterTotalLabel, @ShowFooterAverage, @FooterAverageLabel, @ApplyInFilter1, @ApplyInFilter2, @ApplyInFilter3,
	@Groupable,@IsDefaultGroup, @ShowGroupTotal,@JoinInfoId, @JoinInfo2Id,@JoinInfo3Id,@JoinInfo4Id,@JoinInfo5Id,@SortingType);

	-- Update Orders or other items
    SELECT SCOPE_IDENTITY();
END
GO

ALTER PROCEDURE [dbo].[SP_Report_TableColumn_Update](
    @Id  INT,
	@DynamicReportInfoId INT,
    @DisplayName NVARCHAR(255),
    @FieldName NVARCHAR(255),
    @FieldWithPrefix NVARCHAR(1000),
    @IsVisible BIT,
    @Filterable BIT,
    @Sortable BIT,
    @TextAlign SMALLINT,
    @DisplayOrder INT,
    @Width NVARCHAR(10),
    @TextColor NVARCHAR(50),
    @FieldType SMALLINT,
    @DispalyFormat varchar(100),
    @BackgroundColor VARCHAR(200),
	@BackgroundColorRules varchar(200),
    @ShowFooterTotal BIT,
    @FooterTotalLabel VARCHAR(100),
    @ShowFooterAverage BIT,
    @FooterAverageLabel VARCHAR(100),
    @ApplyInFilter1 BIT,
    @ApplyInFilter2 BIT,
    @ApplyInFilter3 BIT,
	@Groupable BIT,
    @IsDefaultGroup BIT,
	@ShowGroupTotal BIT,
	@JoinInfoId INT,
	@JoinInfo2Id INT,
	@JoinInfo3Id INT,
	@JoinInfo4Id INT,
	@JoinInfo5Id INT,
	@SortingType SMALLINT,
	@CreatedDate DATETIME,
    @CreatedByContactId INT
) 
AS
BEGIN  
   SET NOCOUNT ON;

   IF EXISTS (SELECT 1 FROM [dbo].Report_TableColumn WHERE DynamicReportInfoId = @DynamicReportInfoId AND DisplayOrder = @DisplayOrder AND Id <> @Id)
    BEGIN
	   UPDATE [dbo].Report_TableColumn SET DisplayOrder = DisplayOrder + 1  WHERE DynamicReportInfoId = @DynamicReportInfoId AND DisplayOrder >= @DisplayOrder
	END

    UPDATE [dbo].[Report_TableColumn]
    SET 
        DisplayName = @DisplayName,
        FieldName = @FieldName,
        FieldWithPrefix = @FieldWithPrefix,
        IsVisible = @IsVisible,
        Filterable = @Filterable,
        Sortable = @Sortable,
        TextAlign = @TextAlign,
        DisplayOrder = @DisplayOrder,
        Width = @Width,
        TextColor = @TextColor,
        FieldType = @FieldType,
		DispalyFormat = @DispalyFormat,
        BackgroundColor = @BackgroundColor,
		BackgroundColorRules = @BackgroundColorRules,
        ShowFooterTotal = @ShowFooterTotal,
        FooterTotalLabel = @FooterTotalLabel,
        ShowFooterAverage = @ShowFooterAverage,
        FooterAverageLabel = @FooterAverageLabel,
        ApplyInFilter1 = @ApplyInFilter1,
        ApplyInFilter2 = @ApplyInFilter2,
        ApplyInFilter3 = @ApplyInFilter3,
		Groupable = @Groupable,
        IsDefaultGroup = @IsDefaultGroup,
		ShowGroupTotal= @ShowGroupTotal,
		JoinInfoId=@JoinInfoId,
		JoinInfo2Id=@JoinInfo2Id,
		JoinInfo3Id=@JoinInfo3Id,
		JoinInfo4Id=@JoinInfo4Id,
		JoinInfo5Id=@JoinInfo5Id,
		SortingType = @SortingType,
		CreatedDate = @CreatedDate,
        CreatedByContactId = @CreatedByContactId
    WHERE Id = @Id;

END
GO

