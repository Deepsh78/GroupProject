using GroupApi.Dto;

namespace GroupApi.Services.Interface
{
    public interface IUserService
    {
        void AddUser(InsertUserDto userData);
        void UpdateUser(InsertUserDto userdata, Guid id);
        GetUserDto GetAllUser();
        GetUserDto GetUserById(Guid id);
    }
}
