
namespace CutOutWiz.Services.Models.Feedback
{
    public class FeedbackOrderItemModel
    {
        public long FeedbackOrderId { get; set; }
        public long ClientOrderId { get; set; }
        public long ClientOrderItemId { get; set; }
        public string Comment { get; set; }
        public string FeedBackImagePath { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }
    }

}
