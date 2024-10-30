Alter table Common_CategoryService
add IsDeleted bit default 0,
 IsActive bit default 0;
go

-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	delete by id and isactive and isdeleted field update
-- =============================================
CREATE PROCEDURE SP_Common_CategoryService_DeleteById
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
	UPDATE Common_Service
    SET
        IsActive = ISNULL(@IsActive, IsActive),
		IsDeleted = ISNULL(@IsDeleted, IsDeleted)

    WHERE
        Id = @Id;
END
GO

-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Get by id common category
-- =============================================
CREATE PROCEDURE SP_Common_CategoryService_GetById
	-- Add the parameters for the stored procedure here
	@Id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM Common_CategoryService with(nolock) WHERE Id = @Id
END
GO

-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Here update Common Category
-- =============================================
CREATE PROCEDURE SP_Common_CategoryService_Insert
    @CommonCategoryId INT,
    @CommonServiceId INT,
    @TimeInMinutes DECIMAL = NULL,
    @PriceInUSD DECIMAL = NULL,
	@IsActive bit,
	@IsDeleted bit
AS
BEGIN
DECLARE @Id int
   INSERT INTO Common_CategoryService (CommonCategoryId, CommonServiceId, TimeInMinutes, PriceInUSD,IsActive,IsDeleted)
    VALUES (@CommonCategoryId, @CommonServiceId, @TimeInMinutes, @PriceInUSD,@IsActive,@IsDeleted);

    -- Get the ID of the inserted record
    SET @Id = SCOPE_IDENTITY();
END;
go
-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Here insert Common Category
-- =============================================
CREATE PROCEDURE SP_Common_CategoryService_Update
    @Id INT,
    @CommonCategoryId INT,
    @CommonServiceId INT,
    @TimeInMinutes DECIMAL = NULL,
    @PriceInUSD DECIMAL = NULL,
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
-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Get Client Categories
-- =============================================
alter PROCEDURE SP_Common_CategoryService_GetAll 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ccs.*, cs.Name as ServiceName, cc.Name as CategoryName FROM Common_CategoryService ccs with(nolock) 
	LEFT JOIN Common_Service cs on cs.Id = ccs.CommonServiceId
	LEFT JOIN Common_Category cc on cc.Id = ccs.CommonCategoryId
	
	where ccs.IsActive = 1 and ccs.IsDeleted = 0;
END
GO
