namespace CutOutWiz.Services.Models.IBRModels
{
    public class IbrBaseResponse<T>
    {
        public T? Results { get; set; }
        public string message { get; set; }
        public string status { get; set; }
        public int status_code { get; set; }
    }
}
