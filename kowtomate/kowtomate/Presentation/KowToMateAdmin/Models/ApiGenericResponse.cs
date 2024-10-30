namespace KowToMateAdmin.Model
{
    public class ApiBaseResponse
    {
        public bool IsSuccess { get; set; }
        public string Notification { get; set; }
    }

    public class ApiGenericResponse<T> : ApiBaseResponse
    {
        public T Result { get; set; }
    }
}
