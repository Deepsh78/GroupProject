using GroupApi.Dto;

namespace GroupApi.Services.Interface
{
    public interface IUserService
    {
        void AddUser(InsertUserDto userData);
    }
}
