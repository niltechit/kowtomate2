CREATE PROCEDURE sp_hr_employeeProfile_update (
  @Id int,
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
  UPDATE HR_EmployeeProfile
  SET
    ContactId = ISNULL(@ContactId, ContactId),
    MonthlySalary = ISNULL(@MonthlySalary, MonthlySalary),
    YearlyBonus = ISNULL(@YearlyBonus, YearlyBonus),
    ShiftId = ISNULL(@ShiftId, ShiftId),
    DayOffMonday = ISNULL(@DayOffMonday, DayOffMonday),
    DayOffTuesday = ISNULL(@DayOffTuesday, DayOffTuesday),
    DayOffWednesday = ISNULL(@DayOffWednesday, DayOffWednesday),
    DayOffThursday = ISNULL(@DayOffThursday, DayOffThursday),
    DayOffFriday = ISNULL(@DayOffFriday, DayOffFriday),
    DayOffSaturday = ISNULL(@DayOffSaturday, DayOffSaturday),
    DayOffSunday = ISNULL(@DayOffSunday, DayOffSunday),
    Gender = ISNULL(@Gender, Gender),
    DateOfBirth = ISNULL(@DateOfBirth, DateOfBirth),
    FullAddress = ISNULL(@FullAddress, FullAddress),
    HireDate = ISNULL(@HireDate, HireDate)
  WHERE Id = @Id;
END;