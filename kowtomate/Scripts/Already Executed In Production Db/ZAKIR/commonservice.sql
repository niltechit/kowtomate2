Alter table Common_Service
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
CREATE PROCEDURE SP_Common_Service_DeleteById
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
CREATE PROCEDURE SP_Common_Service_GetById
	-- Add the parameters for the stored procedure here
	@Id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM Common_Service with(nolock) WHERE Id = @Id
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
CREATE PROCEDURE SP_Common_Service_Update
    @Id INT,
    @Name NVARCHAR(MAX),
    @TimeInMinutes DECIMAL,
    @PriceInUSD DECIMAL,
    @IsActive BIT,
    @UpdatedDate DATETIME,
    @UpdatedByUsername NVARCHAR(MAX)
AS
BEGIN
    UPDATE Common_Service
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
CREATE PROCEDURE SP_Common_Service_Insert
    @Name NVARCHAR(MAX),
    @TimeInMinutes DECIMAL,
    @PriceInUSD DECIMAL,
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
CREATE PROCEDURE SP_Common_Service_GetAll 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM Common_Service with(nolock) where IsActive = 1 and IsDeleted = 0;
END
GO
