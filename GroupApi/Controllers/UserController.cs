using GroupApi.Dto;
using GroupApi.Services;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GroupApi.Controllers
{
    public class UserController(IUserService userService): Controller
    {
        [HttpPost("Add")]
        public IActionResult AddUser([FromBody] InsertUserDto userDto)
        {
            try
            {
                userService.AddUser(userDto);
                return Ok("Added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
