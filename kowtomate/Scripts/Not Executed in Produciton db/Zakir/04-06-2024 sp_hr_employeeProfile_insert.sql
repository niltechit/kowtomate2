-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE PROCEDURE sp_hr_employeeProfile_insert (

  @ContactId int = null,
  @MonthlySalary decimal(10, 2) = null,
  @YearlyBonus decimal(10, 2) = null,
  @ShiftId bigint = null,
  @DayOffMonday bit = null,
  @DayOffTuesday bit = null,
  @DayOffWednesday bit = null,
  @DayOffThursday bit = null,
  @DayOffFriday bit = null,
  @DayOffSaturday bit = null,
  @DayOffSunday bit = null,
  @Gender nvarchar(10) = null,
  @DateOfBirth datetime = null,
  @FullAddress nvarchar(max) = null,
  @HireDate datetime = null
)
AS
BEGIN
  INSERT INTO HR_EmployeeProfile(
    
    ContactId,
    MonthlySalary,
    YearlyBonus,
    ShiftId,
    DayOffMonday,
    DayOffTuesday,
    DayOffWednesday,
    DayOffThursday,
    DayOffFriday,
    DayOffSaturday,
    DayOffSunday,
    Gender,
    DateOfBirth,
    FullAddress,
    HireDate
  )
  VALUES (

    ISNULL(@ContactId, -1),  -- Replace with appropriate default for ContactId
    ISNULL(@MonthlySalary, 0.00),
    ISNULL(@YearlyBonus, 0.00),
    ISNULL(@ShiftId, -1),  -- Replace with appropriate default for ShiftId
    ISNULL(@DayOffMonday, 0),
    ISNULL(@DayOffTuesday, 0),
    ISNULL(@DayOffWednesday, 0),
    ISNULL(@DayOffThursday, 0),
    ISNULL(@DayOffFriday, 0),
    ISNULL(@DayOffSaturday, 0),
    ISNULL(@DayOffSunday, 0),
    ISNULL(@Gender, 'Unknown'),
    ISNULL(@DateOfBirth, '1900-01-01'),  -- Replace with appropriate default for DateOfBirth
    ISNULL(@FullAddress, ''),
    ISNULL(@HireDate, GETDATE())
  );
SELECT SCOPE_IDENTITY()
END;