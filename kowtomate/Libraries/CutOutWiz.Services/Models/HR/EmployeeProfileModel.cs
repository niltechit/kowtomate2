using System.Security.Principal;

namespace CutOutWiz.Services.Models.HR
{
    public class EmployeeProfileModel
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public decimal? MonthlySalary { get; set; }
        public decimal? YearlyBonus { get; set; }
        public long? ShiftId { get; set; }
        public bool DayOffMonday { get; set; }
        public bool DayOffTuesday { get; set; }
        public bool DayOffWednesday { get; set; }
        public bool DayOffThursday { get; set; }
        public bool DayOffFriday { get; set; }
        public bool DayOffSaturday { get; set; }
        public bool DayOffSunday { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string FullAddress { get; set; }
        public DateTime? HireDate { get; set; }
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string ShiftName { get; set; }
    }
}
