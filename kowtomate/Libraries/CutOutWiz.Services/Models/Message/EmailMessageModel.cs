namespace CutOutWiz.Services.Models.Message
{
    public class EmailMessageModel
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Recipents { get; set; }
    }
}
