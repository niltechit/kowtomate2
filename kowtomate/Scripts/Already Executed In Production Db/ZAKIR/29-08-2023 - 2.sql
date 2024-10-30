CREATE TABLE PathReplacement
(
	Id SMALLINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[Level] INT NOT NULL,
	CompanyId INT NOT NULL,
	OldText NVARCHAR(100) NULL,
	NewText NVARCHAR(100) NULL,
	ExecutionOrder decimal(5,2) NULL,
	[Type] INT NOT NULL,
	IsDeleted BIT DEFAULT 0,
	IsActive BIT DEFAULT 0,
	CreatedDate DATETIME NULL,
	UpdatedDate DATETIME NULL,
	[DateFormat] NVARCHAR(100) NULL,
)

GO


CREATE PROCEDURE SP_PathReplacement_Insert
    @Level INT,
    @CompanyId INT,
    @OldText NVARCHAR(100),
    @NewText NVARCHAR(100),
    @Type INT,
    @ExecutionOrder decimal(5,2),
    @IsDeleted BIT = 0,
    @IsActive BIT = 0,
    @CreatedDate DATETIME = NULL,
    @UpdatedDate DATETIME = NULL,
	@DateFormat nvarchar(100)
AS
BEGIN
    INSERT INTO PathReplacement (Level, CompanyId, OldText, NewText, Type, ExecutionOrder, IsDeleted, IsActive, CreatedDate, UpdatedDate,DateFormat)
    VALUES (@Level, @CompanyId, @OldText, @NewText, @Type, @ExecutionOrder, @IsDeleted, @IsActive, GETDATE(), @UpdatedDate,@DateFormat);
	select SCOPE_IDENTITY()
END;

GO

CREATE PROCEDURE SP_PathReplacement_Update
    @Id SMALLINT,
    @Level INT,
    @CompanyId INT,
    @OldText NVARCHAR(100),
    @NewText NVARCHAR(100),
    @Type INT,
    @ExecutionOrder decimal(5,2),
    @IsDeleted BIT,
    @IsActive BIT,
    @UpdatedDate DATETIME,
	@DateFormat nvarchar(100)
AS
BEGIN
    UPDATE PathReplacement
    SET Level = ISNULL(@Level, Level),
        CompanyId = ISNULL(@CompanyId, CompanyId),
        OldText = ISNULL(@OldText, OldText),
        NewText = ISNULL(@NewText, NewText),
        Type = ISNULL(@Type, Type),
        ExecutionOrder = ISNULL(@ExecutionOrder, ExecutionOrder),
        IsDeleted = ISNULL(@IsDeleted, IsDeleted),
        IsActive = ISNULL(@IsActive, IsActive),
        UpdatedDate = ISNULL(@UpdatedDate, UpdatedDate),
		CreatedDate= GETDATE(),
		DateFormat=ISNULL(@DateFormat,DateFormat)
    WHERE Id = @Id;
END;

GO

CREATE PROCEDURE SP_PathReplacement_Delete
    @Id SMALLINT
AS
BEGIN
    UPDATE PathReplacement SET
	IsDeleted = 1
    WHERE Id = @Id;
END;

GO

CREATE PROCEDURE SP_PathReplacement_Gets
    @CompnayId int
AS
BEGIN
    SELECT *  FROM PathReplacement  WHERE CompanyId = @CompnayId order by ExecutionOrder asc;
END;

