USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Common_CategoryService_Update]    Script Date: 1/16/2024 12:42:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Here insert Common Category
-- =============================================
ALTER PROCEDURE [dbo].[SP_Common_CategoryService_Update]
    @Id INT,
    @CommonCategoryId INT,
    @CommonServiceId INT,
    @TimeInMinutes DECIMAL = NULL,
    @PriceInUSD nvarchar(max) = NULL,
	@IsActive bit = null,
	@IsDeleted bit = null
AS
BEGIN
    UPDATE Common_CategoryService
    SET
        CommonCategoryId = ISNULL(@CommonCategoryId, CommonCategoryId),
        CommonServiceId = ISNULL(@CommonServiceId, CommonServiceId),
        TimeInMinutes = ISNULL(@TimeInMinutes, TimeInMinutes),
        PriceInUSD = ISNULL(@PriceInUSD, PriceInUSD),
		IsActive = ISNULL(@IsActive, IsActive),
		IsDeleted = ISNULL(@IsDeleted, IsDeleted)
    WHERE
        Id = @Id;
END;


GO

-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Here update Common Category
-- =============================================
ALTER PROCEDURE [dbo].[SP_Common_CategoryService_Insert]
    @CommonCategoryId INT,
    @CommonServiceId INT,
    @TimeInMinutes nvarchar(max) = NULL,
    @PriceInUSD DECIMAL = NULL,
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
-- Description:	Here update Common Category
-- =============================================
ALTER PROCEDURE [dbo].[SP_Common_Category_Update]
    @Id INT,
    @Name NVARCHAR(MAX),
    @TimeInMinutes DECIMAL,
    @PriceInUSD nvarchar(max),
    @IsActive BIT,
    @UpdatedDate DATETIME,
    @UpdatedByUsername NVARCHAR(MAX)
AS
BEGIN
    UPDATE Common_Category
    SET
        Name = ISNULL(@Name, Name),
        TimeInMinutes = ISNULL(@TimeInMinutes, TimeInMinutes),
        PriceInUSD = ISNULL(@PriceInUSD, PriceInUSD),
        IsActive = ISNULL(@IsActive, IsActive),
        UpdatedDate = ISNULL(@UpdatedDate, UpdatedDate),
        UpdatedByUsername = ISNULL(@UpdatedByUsername, UpdatedByUsername)
    WHERE
        Id = @Id;
END;



GO


-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Here insert Common Category
-- =============================================
ALTER PROCEDURE [dbo].[SP_Common_Category_Insert]
    @Name NVARCHAR(MAX),
    @TimeInMinutes DECIMAL,
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
-- Description:	Here insert Common Category
-- =============================================
ALTER PROCEDURE [dbo].[SP_Client_CategoryService_Update]
    @Id INT,
    @ClientCategoryId INT,
    @CommonServiceId INT,
    @TimeInMinutes nvarchar(max) = NULL,
    @PriceInUSD DECIMAL = NULL,
	@IsActive bit = null,
	@IsDeleted bit = null
	
AS
BEGIN
    UPDATE Client_CategoryService
    SET
        ClientCategoryId = ISNULL(@ClientCategoryId, ClientCategoryId),
        CommonServiceId = ISNULL(@CommonServiceId, CommonServiceId),
        TimeInMinutes = ISNULL(@TimeInMinutes, TimeInMinutes),
        PriceInUSD = ISNULL(@PriceInUSD, PriceInUSD),
		IsActive = ISNULL(@IsActive, IsActive),
		IsDeleted = ISNULL(@IsDeleted, IsDeleted)
    WHERE
        Id = @Id;
END;

GO


-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Here update Common Category
-- =============================================
ALTER PROCEDURE [dbo].[SP_Client_CategoryService_Insert]
    @CommonServiceId INT,
    @ClientCategoryId INT,
    @TimeInMinutes DECIMAL = NULL,
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
ALTER PROCEDURE [dbo].[SP_Client_Category_Update]
    @Id INT,
    @CommonCategoryId INT,
    @ClientCompanyId INT,
    @TimeInMinutes DECIMAL = NULL,
    @PriceInUSD nvarchar(max) = NULL,
	@IsActive bit = null,
	@IsDeleted bit = null
AS
BEGIN
    UPDATE Client_Category
    SET
        CommonCategoryId = ISNULL(@CommonCategoryId, CommonCategoryId),
        ClientCompanyId = ISNULL(@ClientCompanyId, ClientCompanyId),
        TimeInMinutes = ISNULL(@TimeInMinutes, TimeInMinutes),
        PriceInUSD = ISNULL(@PriceInUSD, PriceInUSD),
		IsActive = ISNULL(@IsActive, IsActive),
		IsDeleted = ISNULL(@IsDeleted, IsDeleted)
    WHERE
        Id = @Id;
END;

GO

ALTER PROCEDURE [dbo].[SP_Client_Category_Insert]
    @CommonCategoryId INT,
    @ClientCompanyId INT,
    @TimeInMinutes DECIMAL = NULL,
    @PriceInUSD nvarchar(max) = NULL,
	@IsActive bit,
	@IsDeleted bit,
	@CreatedByUsername nvarchar(50),
	@CreatedDate datetime
AS
BEGIN

   INSERT INTO Client_Category (CommonCategoryId, ClientCompanyId, TimeInMinutes, PriceInUSD,IsActive,IsDeleted,CreatedByUsername,CreatedDate)
    VALUES (@CommonCategoryId, @ClientCompanyId, @TimeInMinutes, @PriceInUSD,@IsActive,@IsDeleted,@CreatedByUsername,@CreatedDate);

    -- Get the ID of the inserted record
SELECT SCOPE_IDENTITY();
END;
