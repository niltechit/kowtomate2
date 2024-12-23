USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Client_CategoryService_GetAll]    Script Date: 1/11/2024 1:31:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 08-01-2023
-- Description:	Get Client Categories
-- =============================================
ALTER PROCEDURE [dbo].[SP_Client_CategoryService_GetAll] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ccs.*, cc.Name as CategoryName,sc.EmployeeId as CreatedByUsername, cmc.Name as CategoryName, cc.Name as ServiceName FROM Client_CategoryService ccs with(nolock) 
	LEFT JOIN Client_Category cs on cs.Id = ccs.ClientCategoryId
	LEFT JOIN Common_Category cmc on cmc.Id = cs.CommonCategoryId
	LEFT JOIN Common_Service cc on cc.Id = ccs.CommonServiceId
	LEFT JOIN Security_User su on su.ObjectId = ccs.CreatedByUsername
	LEFT JOIN Security_Contact sc on sc.Id = su.ContactId
	
	where ccs.IsActive = 1 and ccs.IsDeleted = 0;
END

delete  from Client_Category
delete from Client_CategoryService
delete from Common_Category
delete from Common_Service
delete from Common_CategoryService

-- Permission list 
--Common.CanViewCommonServicesList
--Common.CanViewCommonCategoryServicesList
--Common.CanViewCommonCategoryList
--Common.CanViewClientCategoryServicesList
--Common.CanViewClientCategoryList