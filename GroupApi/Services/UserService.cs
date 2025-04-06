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

        public void UpdateUser(InsertUserDto userdata, Guid id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(x => x.Id == id);
                if (user == null)
                {
                    throw new Exception("No user found");
                }

                user.FirstName = userdata.FirstName;
                user.LastName = userdata.LastName;
                user.ImageUrl = userdata.ImageUrl;
                user.Email = userdata.Email;
                user.Gender = userdata.Gender;
                

                _context.Users.Update(user);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                
                throw new Exception($"Error updating user: {ex.Message}");
            }
        }

        public GetUserDto GetAllUser()
        {
            try
            {
                var users = _context.Users.ToList();
                if (users == null || !users.Any())
                {
                    throw new Exception("No user found");
                }

                var result = new GetUserDto();
                foreach (var item in users)
                {
                    result = new GetUserDto()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        ImageUrl = item.ImageUrl,
                        IsActive = item.IsActive,
                        RegisterData = item.RegisterData.ToString(),
                        Email = item.Email,
                        Gender = item.Gender
                    };
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public GetUserDto GetUserById(Guid id)
        {
            try
            {
                var users = _context.Users.FirstOrDefault(x => x.Id == id);
                if (users == null)
                {
                    throw new Exception("No user found");
                }
                var result = new GetUserDto()
                {
                    FirstName = users.FirstName,
                    LastName = users.LastName,
                    ImageUrl = users.ImageUrl,
                    IsActive = users.IsActive,
                    RegisterData = users.RegisterData.ToString(),
                    Email = users.Email,
                    Gender = users.Gender
                };
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
 }
