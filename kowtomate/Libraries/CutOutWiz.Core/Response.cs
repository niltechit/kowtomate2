namespace CutOutWiz.Core
{
    public class Response<T>
    {
        public T? Result { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
        public T FileServerId { get; set; }
    }
}
