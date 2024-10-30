Alter table Client_CategoryService
add IsDeleted bit default 0,
IsActive bit default 0
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
CREATE PROCEDURE SP_Client_CategoryService_DeleteById
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
CREATE PROCEDURE SP_Client_CategoryService_GetById
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
CREATE PROCEDURE SP_Client_CategoryService_Insert
    @CommonServiceId INT,
    @ClientCategoryId INT,
    @TimeInMinutes DECIMAL = NULL,
    @PriceInUSD DECIMAL = NULL,
	@IsActive bit,
	@IsDeleted bit,
	@CreatedByUsername nvarchar(50),
	@CreatedDate datetime
AS
BEGIN
DECLARE @Id int
   INSERT INTO Client_CategoryService (CommonServiceId, ClientCategoryId, TimeInMinutes, PriceInUSD,IsActive,IsDeleted,CreatedByUsername,CreatedDate)
    VALUES (@CommonServiceId, @ClientCategoryId, @TimeInMinutes, @PriceInUSD,@IsActive,@IsDeleted,@CreatedByUsername,@CreatedDate);

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
CREATE PROCEDURE SP_Client_CategoryService_Update
    @Id INT,
    @ClientCategoryId INT,
    @CommonServiceId INT,
    @TimeInMinutes DECIMAL = NULL,
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
create PROCEDURE [dbo].[SP_Client_CategoryService_GetAll] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ccs.*, cc.Name as CategoryName,sc.EmployeeId as CreatedByUsername  FROM Client_CategoryService ccs with(nolock) 
	LEFT JOIN Client_Category cs on cs.Id = ccs.ClientCategoryId
	LEFT JOIN Common_Service cc on cc.Id = ccs.CommonServiceId
	LEFT JOIN Security_User su on su.ObjectId = ccs.CreatedByUsername
	LEFT JOIN Security_Contact sc on sc.Id = su.ContactId
	
	where ccs.IsActive = 1 and ccs.IsDeleted = 0;
END