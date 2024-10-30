
namespace CutOutWiz.Services.Models.ApprovalTool
{
    public class ImageMoveRequestModel
    {
        public string? ActionType { get; set; }  //accepted, rejected
        public string? SelectedImages { get; set; }
        public string? Comments { get; set; }
        //public string 
    }
}
