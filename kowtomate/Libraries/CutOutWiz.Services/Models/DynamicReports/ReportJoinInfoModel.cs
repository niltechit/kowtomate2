using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.DynamicReports
{
    public class ReportJoinInfoModel
    {
        public int Id { get; set; }
        public int DynamicReportInfoId { get; set; }

        [Required(ErrorMessage ="Join Name is Required")]
        public string JoinName { get; set;}

        [Required(ErrorMessage = "Join Script is Required")]
        public string JoinScript { get; set;}

        [Required(ErrorMessage = "Display is Required")]
        public int DisplayOrder { get; set; }
    }
}
