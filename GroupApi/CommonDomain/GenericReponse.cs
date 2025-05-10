namespace GroupApi.CommonDomain
{
    public class GenericResponse<T> : Response
    {
        public T? Data { get; set; }

        public static implicit operator GenericResponse<T>(T data)
        {
            return new GenericResponse<T> { Data = data };
        }

        public static implicit operator GenericResponse<T>(ErrorModel data)
        {
            return new GenericResponse<T> { Error = data };
        }
    }
}
