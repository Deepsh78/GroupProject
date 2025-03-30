using GroupApi.Data;
using GroupApi.Dto;
using GroupApi.Entities;
using GroupApi.Services.Interface;

namespace GroupApi.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicaionDbContext _context;

        public void AddUser(InsertUserDto userData)
        {
            try
            {
                var user = new User
                {
                    FirstName = userData.FirstName,
                    LastName = userData.LastName,
                    ImageUrl = userData.ImageUrl,
                    IsActive = userData.IsActive,
                    Gender = userData.Gender,
                    RegisterData = DateTime.Parse(userData.RegisterData),
                    Email = userData.Email,

                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
