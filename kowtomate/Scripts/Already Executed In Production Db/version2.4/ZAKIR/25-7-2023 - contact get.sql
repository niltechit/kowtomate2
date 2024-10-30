USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetById]    Script Date: 7/25/2023 6:12:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[SP_Security_Contact_GetByEmployeeId]
@employeeId int
AS
BEGIN  
	SELECT Id FROM [dbo].[Security_Contact] Where EmployeeId= @employeeId
END