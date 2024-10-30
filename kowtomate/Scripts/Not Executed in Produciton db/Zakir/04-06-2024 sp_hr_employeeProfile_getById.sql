CREATE PROCEDURE sp_hr_employeeProfile_getById (
  @Id int
)
AS
BEGIN
  SELECT [Id]
      ,[ContactId]
      ,[MonthlySalary]
      ,[YearlyBonus]
      ,[ShiftId]
      ,[DayOffMonday]
      ,[DayOffTuesday]
      ,[DayOffWednesday]
      ,[DayOffThursday]
      ,[DayOffFriday]
      ,[DayOffSaturday]
      ,[DayOffSunday]
      ,[Gender]
      ,[DateOfBirth]
      ,[FullAddress]
      ,[HireDate]
  FROM HR_EmployeeProfile
  WHERE Id = @Id;
END;

go


create PROCEDURE [dbo].[sp_hr_employeeProfile_getAll]

AS
BEGIN
  SELECT ep.[Id]
      ,ep.[ContactId]
      ,ep.[MonthlySalary]
      ,ep.[YearlyBonus]
      ,ep.[ShiftId]
      ,ep.[DayOffMonday]
      ,ep.[DayOffTuesday]
      ,ep.[DayOffWednesday]
      ,ep.[DayOffThursday]
      ,ep.[DayOffFriday]
      ,ep.[DayOffSaturday]
      ,ep.[DayOffSunday]
      ,ep.[Gender]
      ,ep.[DateOfBirth]
      ,ep.[FullAddress]
      ,ep.[HireDate]
	  ,sc.EmployeeId EmployeeId
	  ,(sc.FirstName + ' ' + sc.LastName ) Name
	  ,cs.Name ShiftName
  FROM HR_EmployeeProfile ep
  INNER JOIN Security_Contact sc on sc.Id = ep.ContactId
  LEFT JOIN Common_Contact_Shift ccs on ccs.ContactId = sc.Id
  LEFT JOIN Common_Shift cs on cs.Id = ccs.ShiftId
END;


go

CREATE PROCEDURE sp_hr_employeeProfile_deleteById (
  @id nvarchar(500)
)
AS
BEGIN
  DELETE FROM HR_EmployeeProfile
  WHERE Id = CAST(@id AS int);
END;
