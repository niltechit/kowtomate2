
namespace CutOutWiz.Services.Models.Dashboard
{
    public class ImageCountModel
    {
        public int Raw { get; set; }
        public int Completed { get; set; }
        public int Pending { get; set; }
        public int Rejected { get; set; }
    }

    public class MonthlyImageCountModel
    {
        public string LabelName { get; set; }
        public int MonthYearValue { get; set; }
        public string MonthName { get; set; }
        public int TotalImages { get; set; }
        public int TotalImages2 { get; set; }
    }

    public class DatetimeRangeImageCountModel
    {
        public string LabelName { get; set; }
        public int Value { get; set; }
    }


    public class OperationSummaryReportModel
    {
        public int TotalReceivedImages { get; set; }
        public int TotalCompletedImages { get; set; }
        public int TotalPendingImages { get; set; }
        public decimal LeadTimeInMinutes { get; set; }
        public decimal TotalDeadlineOverImagesPercentage { get; set; }
        public int TotalDeadlineOverImages { get; set; }
        //
    }
}
