namespace GroupApi.Services.Interface
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string UserEmail { get; }


    }
}
