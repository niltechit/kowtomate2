Alter table Client_Category
add IsDeleted bit default 0;
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
CREATE PROCEDURE SP_Client_Category_DeleteById
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
CREATE PROCEDURE SP_Client_Category_GetById
	-- Add the parameters for the stored procedure here
	@Id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM Client_Category with(nolock) WHERE Id = @Id
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
ALTER PROCEDURE SP_Client_Category_Insert
    @CommonCategoryId INT,
    @ClientCompanyId INT,
    @TimeInMinutes DECIMAL = NULL,
    @PriceInUSD DECIMAL = NULL,
	@IsActive bit,
	@IsDeleted bit,
	@CreatedByUsername nvarchar(50),
	@CreatedDate datetime
AS
BEGIN
DECLARE @Id int
   INSERT INTO Client_Category (CommonCategoryId, ClientCompanyId, TimeInMinutes, PriceInUSD,IsActive,IsDeleted,CreatedByUsername,CreatedDate)
    VALUES (@CommonCategoryId, @ClientCompanyId, @TimeInMinutes, @PriceInUSD,@IsActive,@IsDeleted,@CreatedByUsername,@CreatedDate);

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
CREATE PROCEDURE SP_Client_Category_Update
    @Id INT,
    @CommonCategoryId INT,
    @ClientCompanyId INT,
    @TimeInMinutes DECIMAL = NULL,
    @PriceInUSD DECIMAL = NULL,
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
ALTER PROCEDURE [dbo].[SP_Client_Category_GetAll] 
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
	
	where ccs.IsActive = 1 and ccs.IsDeleted = 0;
END