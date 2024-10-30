namespace CutOutWiz.Services.Models.HR
{
    public class EmployeeMonthlySalaryHistoryModel
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public decimal? MonthlySalary { get; set; }
        public decimal? OTAmount { get; set; }
        public decimal? BonusAmount { get; set; }
        public decimal? OtherAdditionAmount { get; set; }
        public decimal? OtherDeduction { get; set; }
        public decimal? TotalPayableAmount { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public decimal? OverHeadAmount { get; set; }
        public int? CreatedByContactId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedByContactId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
