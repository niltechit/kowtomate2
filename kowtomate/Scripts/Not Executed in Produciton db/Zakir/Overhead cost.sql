
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
    INSERT INTO [dbo].Accounting_OverheadCost (Id,[Month], [Year], [Amount], [CreatedByContactId], [CreatedDate])
    VALUES (@Month, @Year, @Amount, @CreatedByContactId, @CreatedDate);

    SET @id = SCOPE_IDENTITY();
END;


go

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

go

CREATE PROCEDURE SP_Accounting_OverheadCost_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[Accounting_OverheadCost]
    WHERE [Id] = @Id;
END;


go


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
