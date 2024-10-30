namespace CutOutWiz.Services.Models.EmailSender
{
    public class EmailSendRequestModel
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        //public List<string> Recipents { get; set; }
    }
}
