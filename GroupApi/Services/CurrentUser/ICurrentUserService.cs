namespace GroupApi.Services.CurrentUser
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string UserEmail { get; }


    }
}
