namespace GroupApi.CommonDomain
{
    public class Response
    {
        public ErrorModel? Error { get; set; }
        public bool IsSuccess
        {
            get { return Error == null; }
        }
        public Response()
        {

        }
        public Response(ErrorModel Error)
        {
            this.Error = Error;
        }
        public static implicit operator Response(ErrorModel error)
        {
            return new Response(error);
        }
    }
}
