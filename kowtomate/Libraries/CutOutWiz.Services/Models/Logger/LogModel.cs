
namespace CutOutWiz.Core.Models.Log
{
    public class LogModel
    {
        public string DateRange { get; set; }
        public string Username { get; set; }
        public List<string> Logs { get; set; } = new List<string>();
    }
}
