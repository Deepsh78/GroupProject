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

        [HttpPut("Update/{id}")]
        public IActionResult UpdateUser([FromBody] InsertUserDto userDto, Guid id)
        {
            try
            {
                userService.UpdateUser(userDto, id);
                return Ok("Updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetAll")]
        public IActionResult GetAllUser()
        {
            try
            {
                var users = userService.GetAllUser();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetById/{id}")]
        public IActionResult GetUserById(Guid id)
        {
            try
            {
                var user = userService.GetUserById(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteUser(Guid id)
        {
            try
            {
                userService.DeleteUser(id);
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
