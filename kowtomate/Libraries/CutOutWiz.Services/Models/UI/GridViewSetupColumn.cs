
namespace CutOutWiz.Services.Models.UI
{
    public class GridViewSetupColumn
    {
        public int Id { get; set; }
        public int ColoumnId { get; set; }
        public int GridViewSetupId { get; set; }
        public int DisplayOrder { get; set; }
        public double Width { get; set; }
    }


    public class GridViewSetupColumnSlim
    {
        public int ColoumnId { get; set; }
        public int DisplayOrder { get; set; }
        public double Width { get; set; }

        public string WidthPx => $"{Width}px";
    }
}
