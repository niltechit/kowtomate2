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
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE sp_hr_employeeLeave_getAll
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT el.[Id]
      ,el.[LeaveForContactId]
      ,el.[LeaveTypeId]
      ,el.[LeaveSubTypeId]
      ,el.[LeaveStartRequestDate]
      ,el.[LeaveEndRequestDate]
      ,el.[LeaveApprovedStartDate]
      ,el.[LeaveApprovedEndDate]
      ,el.[EmployeeNote]
      ,el.[HRNote]
      ,el.[LeaveStatus]
      ,el.[LeaveRequestByContactId]
      ,el.[LeaveApprovalByContactId]
	  ,sc.EmployeeId
	  ,(sc.FirstName + ' ' + sc.LastName) as Name
	  FROM [dbo].[HR_EmployeeLeave] el
	  INNER JOIN Security_Contact sc on sc.Id = el.LeaveForContactId
END
GO
