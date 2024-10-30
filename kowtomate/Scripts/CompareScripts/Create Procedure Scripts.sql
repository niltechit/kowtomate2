-- =======================
-- Author:		Md Aminul
-- Create date: 16 August 2022
-- Description: Get Products
-- EXEC [dbo].[BS_SP_PIM_CommonReportWihPaging_GetByFilter] ''
-- =======================
CREATE PROCEDURE  [dbo].[BS_SP_PIM_CommonReportWihPaging_GetByFilter]
	-- Add the parameters for the stored procedure here	
	@Where NVARCHAR(max)='',
	@IsCalculateTotal BIT='true',
	 --Pagination Parameters 
	@PageNumber INT = 0,
	@PageSize INT = 20,
	--Sorting Parameters
	@SortColumn NVARCHAR(max) = '',
	@SortDirection NVARCHAR(4)='ASC',
	@SelectedColumns NVARCHAR(max)='',
	@ExtraJoin NVARCHAR(max)='',
	@MainQuery NVARCHAR(max) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE 
	@TotalCount INT=0,
	@SQL AS NVARCHAR(MAX),
	@FinalSQL AS NVARCHAR(MAX),		
	@OutPut1 NVARCHAR(1000)

	--get totals if page search
	IF(@IsCalculateTotal='true')
	BEGIN

	SET @SQL = N'
	SELECT 
	@TotalCount = COUNT(*)
	FROM ' + @MainQuery + ' '
    + @ExtraJoin + ' '
	+@Where
		
	SET @OutPut1 = N'@TotalCount INT OUTPUT ';
	EXEC sp_executesql @SQL, @OutPut1, @TotalCount =@TotalCount OUTPUT; 
	END	
	--select possible columns
	EXECUTE 
	('SELECT '	
	+@TotalCount+' TotalCount  
      ' + @SelectedColumns+  '
  FROM ' + @MainQuery + ' '
   + @ExtraJoin + ' '
	+@Where
	+' ORDER BY '+@SortColumn +' '+ @SortDirection+' '
	+'OFFSET '+@PageNumber+' ROWS '
	+'FETCH NEXT '+@PageSize+' ROWS ONLY'
	)
END
GO

-- =======================
-- Author:		Md Aminul
-- Create date: 13 May 2024
-- Description: Get Products
-- EXEC [dbo].[BS_SP_PIM_CommonReportWithoutPaging_GetByFilter] ''
-- =======================
CREATE PROCEDURE  [dbo].[BS_SP_PIM_CommonReportWithoutPaging_GetByFilter]
	-- Add the parameters for the stored procedure here	
	@Where NVARCHAR(max)='',
	@SortColumn NVARCHAR(max) = '',
	@SortDirection NVARCHAR(4)='ASC',
	@SelectedColumns NVARCHAR(max)='',
	@ExtraJoin NVARCHAR(max)='',
	@MainQuery NVARCHAR(max) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE 
	@TotalCount INT=0,
	@SQL AS NVARCHAR(MAX),
	@FinalSQL AS NVARCHAR(MAX),		
	@OutPut1 NVARCHAR(1000)

	--select possible columns
	EXECUTE 
	('SELECT 0 TotalCount  ' + @SelectedColumns+  '
  FROM ' + @MainQuery + ' '
   + @ExtraJoin + ' '
	+@Where
	+' ORDER BY '+@SortColumn +' '+ @SortDirection
	)
END
GO

CREATE PROCEDURE SP_Accounting_OverheadCost_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[Accounting_OverheadCost]
    WHERE [Id] = @Id;
END;
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 26-03-2024
-- Description:	Get get Overhead 
CREATE PROCEDURE [dbo].[SP_Accounting_OverheadCost_GetAll]
AS
BEGIN  
    SELECT 
        ovc.[Id],
        DATENAME(MONTH, DATEADD(MONTH, ovc.[Month] - 1, CAST('1900-01-01' AS DATE))) AS [Month],
        ovc.[Year],
        ovc.[Amount],
        sc.FirstName AS CreatedByUsername,
        ovc.[CreatedDate],
        ovc.[UpdatedByContactId],
        ovc.[UpdatedDate]
    FROM [dbo].[Accounting_OverheadCost] ovc
    INNER JOIN Security_Contact sc ON sc.Id = ovc.CreatedByContactId OR sc.Id = ovc.UpdatedByContactId
END
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 26-03-2024
-- Description:	Get Overhead cost by id
CREATE PROCEDURE [dbo].[SP_Accounting_OverheadCost_GetById]
@id int
AS
BEGIN  
	SELECT *
  FROM [dbo].[Accounting_OverheadCost] Where Id = @id

END
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 20-03-2024
-- Description:	Insert Data 
-- =============================================
CREATE PROCEDURE SP_Accounting_OverheadCost_Insert
    @Month INT,
    @Year INT,
    @Amount DECIMAL(18, 2),
    @CreatedByContactId INT,
    @CreatedDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @id int
    INSERT INTO [dbo].Accounting_OverheadCost ([Month], [Year], [Amount], [CreatedByContactId], [CreatedDate])
    VALUES (@Month, @Year, @Amount, @CreatedByContactId, @CreatedDate);

    SET @id = SCOPE_IDENTITY();
END;
GO

CREATE PROCEDURE SP_Accounting_OverheadCost_Update
    @Id INT,
    @Month INT,
    @Year INT,
    @Amount DECIMAL(18, 2),
    @UpdatedByContactId INT,
    @UpdatedDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Accounting_OverheadCost]
    SET [Month] = @Month,
        [Year] = @Year,
        [Amount] = @Amount,
        [UpdatedByContactId] = @UpdatedByContactId,
        [UpdatedDate] = @UpdatedDate
    WHERE [Id] = @Id;
END;
GO

-- =============================================
-- Author:		Md Rakib 
-- Create date: 07-06-2024
-- Description:	Get by Company Id
-- =============================================
CREATE PROCEDURE [dbo].[SP_Client_Category_GetByCompanyId]
	-- Add the parameters for the stored procedure here
	@companyId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT cc.Id,cc.PriceInUSD as ClientCategoryPrice,cc.TimeInMinutes,cmc.Name as CategoryName,cc.CommonCategoryId as CommonCategoryId,cc.ClientCompanyId FROM Client_Category as cc with(nolock) 
	inner join dbo.Common_Category as cmc on cmc.Id = cc.CommonCategoryId
	WHERE ClientCompanyId = @companyId
END
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_client_category_getCategoryServicesByCategoryId] 
	-- Add the parameters for the stored procedure here
	@categoryId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
	SELECT 
	cs.Id,
	cs.Name, 
	--ccs.CommonServiceId as Id,
	ccs.ClientCategoryId,
	ccs.TimeInMinutes,
	ccs.PriceInUSD
	FROM Client_CategoryService ccs
	INNER JOIN Common_Service cs on cs.Id = ccs.CommonServiceId
	WHERE ccs.ClientCategoryId = @categoryId and ccs.IsDeleted = 0

END
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	delete by id and isactive and isdeleted field update
-- =============================================
Create PROCEDURE [dbo].[SP_Client_CategoryService_DeleteByServiceIdAndClientCategoryId]
	-- Add the parameters for the stored procedure here
	@Id int,
	@clientCategoryId int,
	@IsActive BIT = null,
	@IsDeleted bit = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE Client_CategoryService
    SET
        IsActive = ISNULL(@IsActive, IsActive),
		IsDeleted = ISNULL(@IsDeleted, IsDeleted)

    WHERE
        CommonServiceId = @Id and ClientCategoryId = @clientCategoryId
END
GO

-- =============================================
-- Author:		<Rakib>
-- Create date: <24-june-24>
-- Description:	<SP_ClientCategoryBaseRule_GetAll>
-- =============================================
CREATE PROCEDURE [dbo].[SP_ClientCategoryBaseRule_GetAll]
	
AS
BEGIN
	
	SET NOCOUNT ON;

	SELECT ccb.Id,ccb.Name,ccb.CompanyId,ccb.ClientCategoryId,ccb.IsActive,ccb.CreatedDate,
	ccb.CreatedByContactId,ccb.UpdatedDate
	,ccb.UpdatedByContactId,ccb.IsDeleted, cc.Name as CompanyName , common_cat.Name as CategoryName
    FROM ClientCategoryBaseRule as ccb
	inner join dbo.Common_Company cc on ccb.CompanyId = cc.Id
	inner join dbo.Client_Category ccat on ccat.Id = ccb.ClientCategoryId
	inner join dbo.Common_Category common_cat on common_cat.Id = ccat.CommonCategoryId
	where ccb.IsDeleted IS NULL OR ccb.IsDeleted <> 1
END
GO

-- =============================================
-- Author:		<Rakib>
-- Create date: <24-june-24>
-- Description:	<[SP_ClientCategoryBaseRule_GetAllByCompanyId]>
-- =============================================
CREATE PROCEDURE [dbo].[SP_ClientCategoryBaseRule_GetAllByCompanyId]
	@companyId int
AS
BEGIN
	
	SET NOCOUNT ON;

	SELECT *
    FROM ClientCategoryBaseRule where CompanyId = @companyId and (IsDeleted IS NULL OR IsDeleted <> 1)
END
GO

-- =============================================
-- Author:		<Rakib>
-- Create date: <24-june-24>
-- Description:	<[SP_ClientCategoryBaseRule_GetAllById]>
-- =============================================
CREATE PROCEDURE [dbo].[SP_ClientCategoryBaseRule_GetAllById]
	@id int
AS
BEGIN
	
	SET NOCOUNT ON;

	SELECT *
    FROM ClientCategoryBaseRule where Id = @id
END
GO

-- =============================================
-- Author:		<Rakib>
-- Create date: <24 June 2024>
-- Description:	<SP_ClientCategoryBaseRule_Insert>
-- =============================================
CREATE PROCEDURE SP_ClientCategoryBaseRule_Insert
	@CompanyId INT,
    @Name NVARCHAR(255),
    @ClientCategoryId INT,
    @IsActive BIT,
    @CreatedDate DATETIME,
    @CreatedByContactId int
   

AS
BEGIN
	 INSERT INTO [ClientCategoryBaseRule] (
        [CompanyId], 
        [Name], 
        [ClientCategoryId], 
        [IsActive], 
        [CreatedDate], 
        [CreatedByContactId])
    VALUES (
        @CompanyId, 
        @Name, 
        @ClientCategoryId, 
        @IsActive, 
        @CreatedDate, 
        @CreatedByContactId
       );
    
    -- Optionally, you can return the newly inserted record's ID
    SELECT SCOPE_IDENTITY() AS NewId;
END
GO

-- =============================================
-- Author:		<Rakib>
-- Create date: <24 June 2024>
-- Description:	<SP_ClientCategoryBaseRule_Update>
-- =============================================
CREATE PROCEDURE SP_ClientCategoryBaseRule_Update
    @Id INT,
    @CompanyId INT,
    @Name NVARCHAR(255),
    @ClientCategoryId INT,
    @IsActive BIT,
    @UpdatedDate DATETIME = NULL,
    @UpdatedByContactId INT = NULL,
    @IsDeleted BIT = NULL
AS
BEGIN
    UPDATE [ClientCategoryBaseRule]
    SET 
        [CompanyId] = @CompanyId,
        [Name] = @Name,
        [ClientCategoryId] = @ClientCategoryId,
        [IsActive] = @IsActive,
        [UpdatedDate] = @UpdatedDate,
        [UpdatedByContactId] = @UpdatedByContactId,
        [IsDeleted] = @IsDeleted
    WHERE
        [Id] = @Id;
END
GO

-- =============================================
-- Author:	Rakib	
-- Create date: 6 10 2024
-- Description:	Save ActivityLog info 
-- =============================================

cREATE PROCEDURE [dbo].[SP_ClientCategoryChangeLog_Insert](
            
       @ClientCategoryId int,
       @CategorySetByContactId int,
       @CategorySetDate datetime,
       @ClientOrderItemId bigint
)
AS
BEGIN  
    INSERT INTO [dbo].[ClientCategoryChangeLog]
           (
			ClientCategoryId,
			CategorySetByContactId,
			CategorySetDate,
			ClientOrderItemId
           )
     VALUES
          (
		    @ClientCategoryId,
			@CategorySetByContactId,
			@CategorySetDate,
			@ClientOrderItemId
		   )

	SELECT SCOPE_IDENTITY();
END
GO

-- =============================================
-- Author:		<Rakib>
-- Create date: <28-june-24>
-- Description:	<[SP_ClientCategoryRule_GetAllByCategorygBaseRuleId]>
-- =============================================
CREATE PROCEDURE [dbo].[SP_ClientCategoryRule_GetAllByCategorygBaseRuleId]
	@clientCategoryBaseRuleId int
AS
BEGIN
	
	SET NOCOUNT ON;

	SELECT *
    FROM ClientCategoryRule where ClientCategoryBaseRuleId = @clientCategoryBaseRuleId and (IsDeleted IS NULL OR IsDeleted <> 1)
End
GO

-- =============================================
-- Author:		<Rakib>
-- Create date: <28-june-24>
-- Description:	<[SP_ClientCategoryRule_GetAllById]>
-- =============================================
Create PROCEDURE [dbo].[SP_ClientCategoryRule_GetAllById]
	@id int
AS
BEGIN
	
	SET NOCOUNT ON;

	SELECT *
    FROM ClientCategoryRule where Id = @id
END
GO

-- =============================================
-- Author:		<Rakib>
-- Create date: <27 June 2024>
-- Description:	<[SP_ClientCategoryRule_Insert]>
-- =============================================
CREATE PROCEDURE [dbo].[SP_ClientCategoryRule_Insert]
    @Name NVARCHAR(255),
    @ClientCategoryBaseRuleId INT,
    @Label NVARCHAR(255),
    @Indicator NVARCHAR(50),
    @FilterType NVARCHAR(50),
    @ExecutionOrder INT,
    @IsActive BIT,
    @CreatedDate DATETIME,
    @CreatedByContactId INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [KowToMateERP_Dev].[dbo].[ClientCategoryRule] (
        [Name],
        [ClientCategoryBaseRuleId],
        [Label],
        [Indicator],
        [FilterType],
        [ExecutionOrder],
        [IsActive],
        [CreatedDate],
        [CreatedByContactId]
       
    )
    VALUES (
        @Name,
        @ClientCategoryBaseRuleId,
        @Label,
        @Indicator,
        @FilterType,
        @ExecutionOrder,
        @IsActive,
        @CreatedDate,
        @CreatedByContactId
    );
END;
GO

-- =============================================
-- Author:		<Rakib>
-- Create date: <28 June 2024>
-- Description:	<[SP_ClientCategoryRule_Update]>
-- =============================================
Create PROCEDURE [dbo].[SP_ClientCategoryRule_Update]
    @Id INT,
	@Name NVARCHAR(255),
    @Label INT,
    @Indicator NVARCHAR(255),
	@FilterType tinyint,
    @ExecutionOrder INT,
    @IsActive BIT,
    @UpdatedDate DATETIME = NULL,
    @UpdatedByContactId INT = NULL,
    @IsDeleted BIT = NULL
AS
BEGIN
    UPDATE [ClientCategoryRule]
    SET
	    [Name] = @Name, 
        [Label] = @Label,
        [Indicator] = @Indicator,
		[FilterType] = @FilterType,
		[ExecutionOrder] = @ExecutionOrder,
        [IsActive] = @IsActive,
        [UpdatedDate] = @UpdatedDate,
        [UpdatedByContactId] = @UpdatedByContactId,
        [IsDeleted] = @IsDeleted
    WHERE
        [Id] = @Id;
END
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_clientCategoryService_getByCategoryId]
	-- Add the parameters for the stored procedure here
	@categoryId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	--SELECT CCS.*

	--FROM [dbo].Client_Category CC
	--LEFT JOIN Common_Category CCAT ON CCAT.Id = CC.CommonCategoryId 
	--LEFT JOIN Common_CategoryService CCS ON CCS.CommonCategoryId= CCAT.Id
	--LEFT JOIN Common_Service CS ON CS.Id = CCS.CommonServiceId
	--WHERE CC.CommonCategoryId = @categoryId

	SELECT CS.Name,CS.Id, CCS.PriceInUSD,CCS.TimeInMinutes

	FROM  Common_Category CCAT
	LEFT JOIN Common_CategoryService CCS ON CCS.CommonCategoryId= CCAT.Id
	LEFT JOIN Common_Service CS ON CS.Id = CCS.CommonServiceId
	WHERE CCS.CommonCategoryId = @categoryId
END
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Get Client Categories
-- =============================================
CREATE PROCEDURE [dbo].[SP_Common_Category_GetAll_by_CategoryId] 
	-- Add the parameters for the stored procedure here
	@categoryId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM Common_CategoryService with(nolock) where IsDeleted = 0 AND CommonCategoryId =@categoryId;
END
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_common_category_getServices_by_CategoryId]
	-- Add the parameters for the stored procedure here
	@categoryId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
	SELECT cs.Id, cs.Name,ccs.TimeInMinutes,ccs.CommonServiceId, ccs.PriceInUSD FROM Common_CategoryService ccs

	INNER JOIN Common_Category cc on cc.Id = ccs.CommonCategoryId
	INNER JOIN Common_Service cs on cs.Id = ccs.CommonServiceId
	WHERE ccs.CommonCategoryId = @categoryId and ccs.IsDeleted = 0

END
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Get Client Categories
-- =============================================
CREATE PROCEDURE [dbo].[SP_Common_Category_Service_GetAll_by_CategoryId] 
	-- Add the parameters for the stored procedure here
	@categoryId int 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM Common_CategoryService with(nolock) where IsDeleted = 0 AND CommonCategoryId =@categoryId;
END
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	delete by id and isactive and isdeleted field update
-- =============================================
CREATE PROCEDURE [dbo].[SP_Common_CategoryService_DeleteByServiceIdAndCategoryId]
	-- Add the parameters for the stored procedure here
	@ServiceId int,
	@CategoryId int,
	@IsActive BIT,
	@IsDeleted bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE Common_CategoryService
    SET
        --IsActive = ISNULL(@IsActive, IsActive),
		IsDeleted = ISNULL(@IsDeleted, IsDeleted)

    WHERE
        CommonServiceId = @ServiceId and CommonCategoryId = @CategoryId
END
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[sp_common_categoryService_getServiceId_and_CategoryId]
	-- Add the parameters for the stored procedure here
	@categoryId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
	SELECT cs.* FROM Common_CategoryService ccs

	INNER JOIN Common_Category cc on cc.Id = ccs.CommonCategoryId
	INNER JOIN Common_Service cs on cs.Id = ccs.CommonServiceId
	WHERE ccs.CommonCategoryId = @categoryId and ccs.IsDeleted = 0

END
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_common_categoryService_getServiceIdAndCategoryId]
	-- Add the parameters for the stored procedure here
	@categoryId int,
	@ServiceId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
	SELECT ccs.Id,cs.Name,cs.TimeInMinutes FROM Common_CategoryService ccs

	INNER JOIN Common_Category cc on cc.Id = ccs.CommonCategoryId
	INNER JOIN Common_Service cs on cs.Id = ccs.CommonServiceId
	WHERE ccs.CommonCategoryId = @categoryId and ccs.CommonServiceId = @ServiceId

END
GO

-- =======================
-- Author:		Md Aminul
-- Create date: 16 August 2022
-- Description: Get Products
-- EXEC [dbo].[BS_SP_PIM_CommonReportWihPaging_GetByFilter] ''
-- =======================
CREATE PROCEDURE  [dbo].[SP_CommonReportWihPaging_GetByFilter]
	-- Add the parameters for the stored procedure here	
	@Where NVARCHAR(max)='',
	@IsCalculateTotal BIT='true',
	 --Pagination Parameters 
	@PageNumber INT = 0,
	@PageSize INT = 20,
	--Sorting Parameters
	@SortColumn NVARCHAR(max) = '',
	@SortDirection NVARCHAR(4)='ASC',
	@SelectedColumns NVARCHAR(max)='',
	@ExtraJoin NVARCHAR(max)='',
	@MainQuery NVARCHAR(max) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE 
	@TotalCount INT=0,
	@SQL AS NVARCHAR(MAX),
	@FinalSQL AS NVARCHAR(MAX),		
	@OutPut1 NVARCHAR(1000)

	--get totals if page search
	IF(@IsCalculateTotal='true')
	BEGIN

	SET @SQL = N'
	SELECT 
	@TotalCount = COUNT(*)
	FROM ' + @MainQuery + ' '
    + @ExtraJoin + ' '
	+@Where
		
	SET @OutPut1 = N'@TotalCount INT OUTPUT ';
	EXEC sp_executesql @SQL, @OutPut1, @TotalCount =@TotalCount OUTPUT; 
	END	
	--select possible columns
	EXECUTE 
	('SELECT '	
	+@TotalCount+' TotalCount  
      ' + @SelectedColumns+  '
  FROM ' + @MainQuery + ' '
   + @ExtraJoin + ' '
	+@Where
	+' ORDER BY '+@SortColumn +' '+ @SortDirection+' '
	+'OFFSET '+@PageNumber+' ROWS '
	+'FETCH NEXT '+@PageSize+' ROWS ONLY'
	)
END
GO

-- =======================
-- Author:		Md Aminul
-- Create date: 13 May 2024
-- Description: Get Products
-- EXEC [dbo].[BS_SP_PIM_CommonReportWithoutPaging_GetByFilter] ''
-- =======================
CREATE PROCEDURE  [dbo].[SP_CommonReportWithoutPaging_GetByFilter]
	-- Add the parameters for the stored procedure here	
	@Where NVARCHAR(max)='',
	@SortColumn NVARCHAR(max) = '',
	@SortDirection NVARCHAR(4)='ASC',
	@SelectedColumns NVARCHAR(max)='',
	@ExtraJoin NVARCHAR(max)='',
	@MainQuery NVARCHAR(max) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE 
	@TotalCount INT=0,
	@SQL AS NVARCHAR(MAX),
	@FinalSQL AS NVARCHAR(MAX),		
	@OutPut1 NVARCHAR(1000)

	--select possible columns
	EXECUTE 
	('SELECT 0 TotalCount  ' + @SelectedColumns+  '
  FROM ' + @MainQuery + ' '
   + @ExtraJoin + ' '
	+@Where
	+' ORDER BY '+@SortColumn +' '+ @SortDirection
	)
END
GO

CREATE PROCEDURE sp_employee_leave_insert
	@Id int out,
    @LeaveForContactId INT,
    @LeaveTypeId INT,
    @LeaveSubTypeId INT = NULL,
    @LeaveStartRequestDate DATETIME,
    @LeaveEndRequestDate DATETIME,
    @LeaveApprovedStartDate DATETIME = NULL,
    @LeaveApprovedEndDate DATETIME = NULL,
    @EmployeeNote NVARCHAR(MAX),
    @HRNote NVARCHAR(MAX),
    @LeaveStatus NVARCHAR(50),
    @LeaveRequestByContactId INT,
    @LeaveApprovalByContactId INT = NULL
AS
BEGIN
    INSERT INTO HR_EmployeeLeave (
        LeaveForContactId,
        LeaveTypeId,
        LeaveSubTypeId,
        LeaveStartRequestDate,
        LeaveEndRequestDate,
        LeaveApprovedStartDate,
        LeaveApprovedEndDate,
        EmployeeNote,
        HRNote,
        LeaveStatus,
        LeaveRequestByContactId,
        LeaveApprovalByContactId
    )
    VALUES (
        @LeaveForContactId,
        @LeaveTypeId,
        ISNULL(@LeaveSubTypeId, null),  -- Assuming 0 as a default value
        @LeaveStartRequestDate,
        @LeaveEndRequestDate,
        ISNULL(@LeaveApprovedStartDate, '1900-01-01'),  -- Assuming '1900-01-01' as a default value
        ISNULL(@LeaveApprovedEndDate, '1900-01-01'),    -- Assuming '1900-01-01' as a default value
        ISNULL(@EmployeeNote, ''),  -- Assuming empty string as a default value
        ISNULL(@HRNote, ''),        -- Assuming empty string as a default value
        @LeaveStatus,
        @LeaveRequestByContactId,
        ISNULL(@LeaveApprovalByContactId, null)  -- Assuming 0 as a default value
    );

    -- Return the newly inserted ID
    SELECT @Id = SCOPE_IDENTITY()
END;
GO

CREATE PROCEDURE [dbo].[sp_employee_leave_update]
    @Id INT,
    @LeaveForContactId INT,
    @LeaveTypeId INT,
    @LeaveSubTypeId INT = NULL,
    @LeaveStartRequestDate DATETIME,
    @LeaveEndRequestDate DATETIME,
    @LeaveApprovedStartDate DATETIME = NULL,
    @LeaveApprovedEndDate DATETIME = NULL,
    @EmployeeNote NVARCHAR(MAX),
    @HRNote NVARCHAR(MAX),
    @LeaveStatus NVARCHAR(50),
    @LeaveRequestByContactId INT,
    @LeaveApprovalByContactId INT = NULL
AS
BEGIN
    UPDATE HR_EmployeeLeave
    SET
        LeaveForContactId = @LeaveForContactId,
        LeaveTypeId = @LeaveTypeId,
        LeaveSubTypeId = ISNULL(@LeaveSubTypeId, LeaveSubTypeId),  -- Update only if provided, otherwise keep existing
        LeaveStartRequestDate = @LeaveStartRequestDate,
        LeaveEndRequestDate = @LeaveEndRequestDate,
        LeaveApprovedStartDate = ISNULL(@LeaveApprovedStartDate, LeaveApprovedStartDate),  -- Update only if provided, otherwise keep existing
        LeaveApprovedEndDate = ISNULL(@LeaveApprovedEndDate, LeaveApprovedEndDate),        -- Update only if provided, otherwise keep existing
        EmployeeNote = ISNULL(@EmployeeNote, EmployeeNote),  -- Update only if provided, otherwise keep existing
        HRNote = ISNULL(@HRNote, HRNote),                    -- Update only if provided, otherwise keep existing
        LeaveStatus = @LeaveStatus,
        LeaveRequestByContactId = @LeaveRequestByContactId,
        LeaveApprovalByContactId = ISNULL(@LeaveApprovalByContactId, LeaveApprovalByContactId)  -- Update only if provided, otherwise keep existing
    WHERE Id = @Id;
END;
GO

CREATE PROCEDURE sp_EmployeeLeave_GetById
    @id INT
AS
BEGIN
    SELECT * FROM HR_EmployeeLeave WHERE id = @id;
END;
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE sp_hr_employeeLeave_getAll
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT el.[Id]
      ,el.[LeaveForContactId]
      ,el.[LeaveTypeId]
      ,el.[LeaveSubTypeId]
      ,el.[LeaveStartRequestDate]
      ,el.[LeaveEndRequestDate]
      ,el.[LeaveApprovedStartDate]
      ,el.[LeaveApprovedEndDate]
      ,el.[EmployeeNote]
      ,el.[HRNote]
      ,el.[LeaveStatus]
      ,el.[LeaveRequestByContactId]
      ,el.[LeaveApprovalByContactId]
	  ,sc.EmployeeId
	  ,(sc.FirstName + ' ' + sc.LastName) as Name
	  FROM [dbo].[HR_EmployeeLeave] el
	  INNER JOIN Security_Contact sc on sc.Id = el.LeaveForContactId
END
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE sp_hr_employeeLeave_getById 
	-- Add the parameters for the stored procedure here
	@Id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * from HR_EmployeeLeave where Id = @Id
END
GO

CREATE PROCEDURE [sp_hr_employeeProfile_deleteByid] (
  @id nvarchar(500)
)
AS
BEGIN
  DELETE FROM HR_EmployeeProfile
  WHERE Id = CAST(@id AS int);
END;
GO

CREATE PROCEDURE [dbo].[sp_hr_employeeProfile_getAll]

AS
BEGIN
  SELECT ep.[Id]
      ,ep.[ContactId]
      ,ep.[MonthlySalary]
      ,ep.[YearlyBonus]
      ,ep.[ShiftId]
      ,ep.[DayOffMonday]
      ,ep.[DayOffTuesday]
      ,ep.[DayOffWednesday]
      ,ep.[DayOffThursday]
      ,ep.[DayOffFriday]
      ,ep.[DayOffSaturday]
      ,ep.[DayOffSunday]
      ,ep.[Gender]
      ,ep.[DateOfBirth]
      ,ep.[FullAddress]
      ,ep.[HireDate]
	  ,sc.EmployeeId EmployeeId
	  ,(sc.FirstName + ' ' + sc.LastName ) Name
	  ,cs.Name ShiftName
  FROM HR_EmployeeProfile ep
  INNER JOIN Security_Contact sc on sc.Id = ep.ContactId
  LEFT JOIN Common_Contact_Shift ccs on ccs.ContactId = sc.Id
  LEFT JOIN Common_Shift cs on cs.Id = ccs.ShiftId
END;
GO

CREATE PROCEDURE sp_hr_employeeProfile_getById (
  @Id int
)
AS
BEGIN
  SELECT [Id]
      ,[ContactId]
      ,[MonthlySalary]
      ,[YearlyBonus]
      ,[ShiftId]
      ,[DayOffMonday]
      ,[DayOffTuesday]
      ,[DayOffWednesday]
      ,[DayOffThursday]
      ,[DayOffFriday]
      ,[DayOffSaturday]
      ,[DayOffSunday]
      ,[Gender]
      ,[DateOfBirth]
      ,[FullAddress]
      ,[HireDate]
  FROM HR_EmployeeProfile
  WHERE Id = @Id;
END;
GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE PROCEDURE sp_hr_employeeProfile_insert (

  @ContactId int = null,
  @MonthlySalary decimal(10, 2) = null,
  @YearlyBonus decimal(10, 2) = null,
  @ShiftId bigint = null,
  @DayOffMonday bit = null,
  @DayOffTuesday bit = null,
  @DayOffWednesday bit = null,
  @DayOffThursday bit = null,
  @DayOffFriday bit = null,
  @DayOffSaturday bit = null,
  @DayOffSunday bit = null,
  @Gender nvarchar(10) = null,
  @DateOfBirth datetime = null,
  @FullAddress nvarchar(max) = null,
  @HireDate datetime = null
)
AS
BEGIN
  INSERT INTO HR_EmployeeProfile(
    
    ContactId,
    MonthlySalary,
    YearlyBonus,
    ShiftId,
    DayOffMonday,
    DayOffTuesday,
    DayOffWednesday,
    DayOffThursday,
    DayOffFriday,
    DayOffSaturday,
    DayOffSunday,
    Gender,
    DateOfBirth,
    FullAddress,
    HireDate
  )
  VALUES (

    ISNULL(@ContactId, -1),  -- Replace with appropriate default for ContactId
    ISNULL(@MonthlySalary, 0.00),
    ISNULL(@YearlyBonus, 0.00),
    ISNULL(@ShiftId, -1),  -- Replace with appropriate default for ShiftId
    ISNULL(@DayOffMonday, 0),
    ISNULL(@DayOffTuesday, 0),
    ISNULL(@DayOffWednesday, 0),
    ISNULL(@DayOffThursday, 0),
    ISNULL(@DayOffFriday, 0),
    ISNULL(@DayOffSaturday, 0),
    ISNULL(@DayOffSunday, 0),
    ISNULL(@Gender, 'Unknown'),
    ISNULL(@DateOfBirth, '1900-01-01'),  -- Replace with appropriate default for DateOfBirth
    ISNULL(@FullAddress, ''),
    ISNULL(@HireDate, GETDATE())
  );
SELECT SCOPE_IDENTITY()
END;
GO

CREATE PROCEDURE sp_hr_employeeProfile_update (
  @Id int,
  @ContactId int = null,
  @MonthlySalary decimal(10, 2) = null,
  @YearlyBonus decimal(10, 2) = null,
  @ShiftId bigint = null,
  @DayOffMonday bit = null,
  @DayOffTuesday bit = null,
  @DayOffWednesday bit = null,
  @DayOffThursday bit = null,
  @DayOffFriday bit = null,
  @DayOffSaturday bit = null,
  @DayOffSunday bit = null,
  @Gender nvarchar(10) = null,
  @DateOfBirth datetime = null,
  @FullAddress nvarchar(max) = null,
  @HireDate datetime = null
)
AS
BEGIN
  UPDATE HR_EmployeeProfile
  SET
    ContactId = ISNULL(@ContactId, ContactId),
    MonthlySalary = ISNULL(@MonthlySalary, MonthlySalary),
    YearlyBonus = ISNULL(@YearlyBonus, YearlyBonus),
    ShiftId = ISNULL(@ShiftId, ShiftId),
    DayOffMonday = ISNULL(@DayOffMonday, DayOffMonday),
    DayOffTuesday = ISNULL(@DayOffTuesday, DayOffTuesday),
    DayOffWednesday = ISNULL(@DayOffWednesday, DayOffWednesday),
    DayOffThursday = ISNULL(@DayOffThursday, DayOffThursday),
    DayOffFriday = ISNULL(@DayOffFriday, DayOffFriday),
    DayOffSaturday = ISNULL(@DayOffSaturday, DayOffSaturday),
    DayOffSunday = ISNULL(@DayOffSunday, DayOffSunday),
    Gender = ISNULL(@Gender, Gender),
    DateOfBirth = ISNULL(@DateOfBirth, DateOfBirth),
    FullAddress = ISNULL(@FullAddress, FullAddress),
    HireDate = ISNULL(@HireDate, HireDate)
  WHERE Id = @Id;
END;
GO

Create PROCEDURE [dbo].SP_Order_ClientOrder_CategorySetStatusUpdate
(
	@Id bigint,
	@CategorySetStatus tinyint
)
as 
begin 
	update Order_ClientOrder
	set
	CategorySetStatus=@CategorySetStatus
	where Id=@Id
end
GO

Create PROCEDURE [dbo].SP_Order_ClientOrderItem_ApprovedCategorySetStatus 
(
	@id bigint,
	@CategorySetStatus tinyint,
	@CategoryApprovedByContactId int

)
as
begin
	 UPDATE [dbo].Order_ClientOrderItem
    SET
		CategorySetStatus = @CategorySetStatus,
		CategoryApprovedByContactId = @CategoryApprovedByContactId
	WHERE Id=@id
end
GO

CREATE PROCEDURE [dbo].[SP_Order_ClientOrderItem_UpdateCategory] 
(
	@id bigint,
	@categoryId int,
	@categorySetByContactId int,
	@categorySetDate datetime,
	@categoryPrice nvarchar(255),
	@TimeInMinute decimal,
	@CategorySetStatus tinyint,
	@CategoryApprovedByContactId int

)
as
begin
	 UPDATE [dbo].Order_ClientOrderItem
    SET
		ClientCategoryId = @categoryId,
		CategorySetByContactId = @categorySetByContactId,
		CategorySetDate = @categorySetDate,
		CategoryPrice = @categoryPrice,
		@TimeInMinute = @TimeInMinute,
		CategorySetStatus = @CategorySetStatus,
		CategoryApprovedByContactId = @CategoryApprovedByContactId
	WHERE Id=@id
end
GO

-- Author:		Md Rakib Hossain
-- Create date: 10-11-2022
-- Description:	SP Order ClientOrder Items Min CategorySet StatusByOrderId
-- =============================================
CREATE PROCEDURE [dbo].[SP_Order_ClientOrderItemsMinCategorySetStatusByOrderId] 
	@OrderId int,
	@FileGroup int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT  Min(CategorySetStatus) CategorySetStatus from dbo.Order_ClientOrderItem 
	WITH(NOLOCK)
	where ClientOrderId=@OrderId and FileGroup = @FileGroup

END
GO

-- =============================================
-- Author:		Aminul
-- Create date: 13 July 2023
-- Description:	Get Field By Name
-- [dbo].[SP_Report_TableColumn_GetByFieldName] 'LRCPTQTY'
-- =============================================
CREATE PROCEDURE [dbo].[SP_Report_TableColumn_GetByFieldName]
@FieldName varchar(255)
AS
BEGIN  
	SELECT top 1  * 
  FROM [dbo].[Report_TableColumn] WITH(NOLOCK) WHERE FieldName = @FieldName ORDER BY Id DESC
END
GO

-- =============================================
-- Author:		Md Aminul
-- Create date: 25 Sep 2022
-- Description:	Delete Grid View Filter
-- [dbo].[SP_UI_GridViewFilter_Delete] 3
-- =============================================
CREATE PROCEDURE [dbo].[SP_UI_GridViewFilter_Delete]
(
    @GridVewFilterId  INT  
)
AS
BEGIN  
   DELETE FROM  [dbo].[UI_GridVewFilter] WHERE Id = @GridVewFilterId
END
GO

-- =============================================
-- Author:		Md Aminul
-- Create date: 25 Sep 2022
-- Description:	Get Grid View Filter by setup Id
-- [dbo].[SP_UI_GridViewFilter_GetListByGridViewSetupId] 1, 2
-- =============================================
CREATE PROCEDURE [dbo].[SP_UI_GridViewFilter_GetListByGridViewSetupId]
@GridViewSetupId INT,
@ContactId INT
AS
BEGIN  
SELECT f.[Id]
      ,f.[GridViewSetupId]
      ,f.[Name]
	  ,(CASE WHEN f.ContactId = @ContactId THEN f.[Name] + ' (Own)' ELSE f.[Name] + ' ('+c.FirstName+')' END )  DisplayName
      ,f.[FilterJson]
      ,f.[IsDefault]
	  ,f.[IsPublic]
	  ,f.[LogicalOperator]
	  ,f.[SortColumn]
	  ,f.[SortOrder]
      ,f.[UpdatedDate]
  FROM [dbo].[UI_GridVewFilter] f WITH(NOLOCK) 
  INNER JOIN [dbo].Security_Contact c on c.Id = f.ContactId
  WHERE 
  f.GridViewSetupId = @GridViewSetupId AND (f.ContactId = @ContactId OR f.IsPublic = 1)
  ORDER BY [Name]
END
GO

-- =============================================
-- Author:		Md Aminul
-- Create date: 23 May 2022
-- Description: Add New Grid View Filter 
-- =============================================
CREATE PROCEDURE [dbo].[SP_UI_GridViewFilter_Insert]
( 
	@GridViewSetupId [int],
	@ContactId [int],
	@Name nvarchar(100),
	@FilterJson [nvarchar] (max),
	@IsDefault [bit],
	@IsPublic [bit],
	@LogicalOperator varchar(5),
	@SortColumn varchar(50),
	@SortOrder varchar(50),
	@UpdatedDate [datetime]
)
AS
BEGIN  

    if(@IsDefault = 'True')
	begin
		UPDATE dbo.UI_GridVewFilter set IsDefault='False' where GridViewSetupId = @GridViewSetupId  AND IsDefault ='True' 
	end

	INSERT INTO [dbo].[UI_GridVewFilter]
           ([GridViewSetupId]
		   ,[ContactId]
		   ,[Name]
           ,[FilterJson]
           ,[IsDefault]
		   ,[IsPublic]
		   ,[LogicalOperator]
			,[SortColumn]
			,[SortOrder]
           ,[UpdatedDate])
     VALUES
           (@GridViewSetupId
           ,@ContactId
		   ,@Name
		   ,@FilterJson
           ,@IsDefault
		   ,@IsPublic
		   ,@LogicalOperator
		   ,@SortColumn 
		   ,@SortOrder
           ,@UpdatedDate
		   )

       SELECT SCOPE_IDENTITY()
END
GO

-- =============================================
-- Author:		Md Aminul
-- Create date: 13 Oct 2022
-- Description: Update Grid View Filter 
-- =============================================
CREATE PROCEDURE [dbo].[SP_UI_GridViewFilter_Update]
( 
    @Id INT,
	@GridViewSetupId [int],
	@Name nvarchar(100),
	@FilterJson [nvarchar] (max),
	@IsDefault [bit],
	@IsPublic [bit],
	@LogicalOperator varchar(5),
	@SortColumn varchar(50),
	@SortOrder varchar(50),
	@UpdatedDate [datetime]
)
AS
BEGIN  

    if(@IsDefault = 'True')
	begin
		UPDATE dbo.UI_GridVewFilter set IsDefault='False' where GridViewSetupId = @GridViewSetupId  AND IsDefault ='True' 
	end

	UPDATE [dbo].[UI_GridVewFilter] SET
		   [Name] = @Name
           ,[FilterJson] = @FilterJson
           ,[IsDefault] = @IsDefault
		   ,[IsPublic]= @IsPublic
		   ,[LogicalOperator] = @LogicalOperator
		   ,[SortColumn] = @SortColumn
		   ,[SortOrder] = @SortOrder
           ,[UpdatedDate] = @UpdatedDate
        WHERE GridViewSetupId = @GridViewSetupId AND Id = @Id

END
GO

-- =============================================
-- Author:		Md Aminul
-- Create date: 25 Sep 2022
-- Description:	Delete GridViewSetup
-- =============================================
CREATE PROCEDURE [dbo].[SP_UI_GridViewSetup_Delete]
(
    @ObjectId  varchar(40)       
)
AS
BEGIN     
    DELETE FROM [dbo].UI_GridVewFilter WHERE GridViewSetupId IN (Select Id FROM dbo.UI_GridViewSetup WHERE ObjectId = @ObjectId)
    DELETE FROM [dbo].[UI_GridViewSetupColumn] WHERE GridViewSetupId IN (Select Id FROM dbo.UI_GridViewSetup WHERE ObjectId = @ObjectId)
    DELETE FROM  [dbo].[UI_GridViewSetup] WHERE ObjectId = @ObjectId
END
GO

-- =============================================
-- Author:		Md Aminul
-- Create date: 25 Sep 2022
-- Description:	Get Grid View Setup
-- Exec: [dbo].[SP_UI_GridViewSetup_GetListByContactId] 1,1
-- =============================================
CREATE PROCEDURE [dbo].[SP_UI_GridViewSetup_GetListByContactId]
@GridViewFor smallint,
@ContactId INT
AS
BEGIN  
SELECT s.[Id]
      ,s.[ContactId]
      ,s.[Name]
	  ,(CASE WHEN s.ContactId = @ContactId THEN s.[Name] + ' (Own)' 	  	  
	  ELSE s.[Name] + ' ('+c.FirstName+')' 	 END )  DisplayName
	  ,s.[GridViewFor]
      ,s.[OrderByColumn]
      ,s.[OrderDirection]
      ,s.[IsDefault]
	  ,s.[IsPublic]
      ,s.[CreatedDate]
      ,s.[CreatedByContactId]
      ,s.[UpdatedDate]
      ,s.[UpdatedByContactId]
      ,s.[ObjectId]
	  ,s.[ViewStateJson]
  FROM [dbo].[UI_GridViewSetup] s
  INNER JOIN [dbo].Security_Contact c on c.Id = s.ContactId
  WHERE s.GridViewFor = @GridViewFor 
  AND (s.ContactId = @ContactId OR s.IsPublic = 1)
  ORDER BY s.[Name]
END
GO

-- =============================================
-- Author:		Md Aminul
-- Create date: 23 May 2022
-- Description: Add New Grid View Setup 
-- =============================================
CREATE PROCEDURE [dbo].[SP_UI_GridViewSetup_Insert]
( 
	@ContactId [int],
	@Name [varchar](100),
	@GridViewFor smallint,
	@OrderByColumn [varchar](100),
	@OrderDirection [varchar](100),
	@IsDefault [bit],
	@IsPublic [bit],
	@CreatedDate [datetime],
	@CreatedByContactId [int],
	@ObjectId [varchar](40),
	@ViewStateJson nvarchar(max),
	@DynamicReportInfoId INT
)
AS
BEGIN  

    IF(@IsDefault = 'True')
	begin
	    IF (@DynamicReportInfoId > 0)
		BEGIN
		   UPDATE dbo.UI_GridViewSetup set IsDefault='False' where DynamicReportInfoId = @DynamicReportInfoId AND IsDefault ='True'
		END
		ELSE
		BEGIN
		    UPDATE dbo.UI_GridViewSetup set IsDefault='False' where GridViewFor = @GridViewFor AND IsDefault ='True'
		END
	END 

	INSERT INTO [dbo].[UI_GridViewSetup]
           ([ContactId]
           ,[Name]
		   ,[GridViewFor]
           ,[OrderByColumn]
           ,[OrderDirection]
           ,[IsDefault]
		   ,[IsPublic]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[ObjectId]
		   ,[ViewStateJson]
		   ,[DynamicReportInfoId])
     VALUES
           (@ContactId
           ,@Name
		   ,@GridViewFor
           ,@OrderByColumn
           ,@OrderDirection
           ,@IsDefault
		   ,@IsPublic
           ,@CreatedDate
           ,@CreatedByContactId
           ,@ObjectId
		   ,@ViewStateJson
		   ,@DynamicReportInfoId
		   )

       SELECT SCOPE_IDENTITY()
END
GO

-- =============================================
-- Author:		Md Aminul
-- Create date: 25 Sep 2022
-- Description: Update Module
-- =============================================
CREATE PROCEDURE [dbo].[SP_UI_GridViewSetup_Update]
( 
	@Name [varchar](100),
	@GridViewFor smallint,
	@OrderByColumn [varchar](100),
	@OrderDirection [varchar](100),
	@IsDefault [bit],
	@IsPublic [bit],
	@UpdatedDate [datetime],
	@UpdatedByContactId [int],
	@ObjectId [varchar](40),
	@ViewStateJson nvarchar(max),
	@DynamicReportInfoId INT
)
AS
BEGIN  

    IF(@IsDefault = 'True')
	begin
	    IF (@DynamicReportInfoId > 0)
		BEGIN
		   UPDATE dbo.UI_GridViewSetup set IsDefault='False' where DynamicReportInfoId = @DynamicReportInfoId AND IsDefault ='True'
		END
		ELSE
		BEGIN
		    UPDATE dbo.UI_GridViewSetup set IsDefault='False' where GridViewFor = @GridViewFor AND IsDefault ='True'

		END
	END 

	UPDATE [dbo].[UI_GridViewSetup]
   SET 
       [Name] = @Name
      ,[OrderByColumn] = @OrderByColumn
      ,[OrderDirection] = @OrderDirection
      ,[IsDefault] = @IsDefault
	  ,[IsPublic]= @IsPublic
      ,[UpdatedDate] = @UpdatedDate
      ,[UpdatedByContactId] = @UpdatedByContactId
	  ,[ViewStateJson] = @ViewStateJson
     WHERE ObjectId = @ObjectId
END
GO

-- =============================================
-- Author:		Md Aminul islam
-- Create date: 25 Sep 2022
-- Description:	Insert or update Grid Template Columns
-- =============================================
CREATE PROCEDURE [dbo].[SP_UI_GridViewSetupColumn_InsertOrUpdateBySetupId]
(
	@GridViewSetupId INT,
	@ColumnIds varchar(5000)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE FROM [dbo].[UI_GridViewSetupColumn] WHERE GridViewSetupId = @GridViewSetupId

	INSERT INTO [dbo].[UI_GridViewSetupColumn]
           ([ColoumnId]
           ,[GridViewSetupId])
           SELECT CONVERT(int,[value]),@GridViewSetupId FROM fnSplit(@ColumnIds,',')
END
GO

