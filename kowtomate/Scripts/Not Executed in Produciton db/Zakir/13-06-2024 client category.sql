USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Client_CategoryService_Update]    Script Date: 6/13/2024 3:34:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
    @TimeInMinutes decimal = NULL,
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

