using System.Net;
using System.Security.Claims;
using GroupApi.DTOs.Auth;
using GroupApi.Entities.Auth;
using GroupApi.Services.CurrentUser;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GroupApi.CommonDomain;
using GroupApi.DTOs.Auth;
using GroupApi.Services.Interface;

namespace GroupApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(IAuthService authService, ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<GenericResponse<bool>>> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid input data"));

            var result = await _authService.RegisterAsync(model);
            if (!result)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Email already exists"));

            return Ok(new GenericResponse<bool> { Data = true });
        }

        [HttpPost("verify-otp")]
        public async Task<ActionResult<GenericResponse<bool>>> VerifyOtp([FromBody] VerifyOtpDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid input data"));

            var result = await _authService.VerifyOtpAsync(model);
            if (!result)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid or expired OTP"));

            return Ok(new GenericResponse<bool> { Data = true });
        }

        [HttpPost("login")]
        public async Task<ActionResult<GenericResponse<string>>> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid input data"));

            var token = await _authService.LoginAsync(model);
            if (token == null)
                return Unauthorized(new ErrorModel(HttpStatusCode.Unauthorized, "Invalid credentials"));

            return Ok(new GenericResponse<string> { Data = token });
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<GenericResponse<bool>>> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid input data"));

            var result = await _authService.ForgotPasswordAsync(model);
            if (!result)
                return NotFound(new ErrorModel(HttpStatusCode.NotFound, "User not found"));

            return Ok(new GenericResponse<bool> { Data = true });
        }

        [HttpPost("verify-password-reset-otp")]
        public async Task<ActionResult<GenericResponse<bool>>> VerifyPasswordResetOtp([FromBody] VerifyOtpDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid input data"));

            var result = await _authService.VerifyPasswordResetOtpAsync(model);
            if (!result)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid or expired OTP"));

            return Ok(new GenericResponse<bool> { Data = true });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<GenericResponse<bool>>> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid input data"));

            var result = await _authService.ResetPasswordAsync(model);
            if (!result)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid request or OTP expired"));

            return Ok(new GenericResponse<bool> { Data = true });
        }

        [HttpPost("resend-otp")]
        public async Task<ActionResult<GenericResponse<bool>>> ResendOtp([FromBody] ResendOtpDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid input data"));

            var result = await _authService.ResendOtpAsync(model);
            if (!result)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid request or user not found"));

            return Ok(new GenericResponse<bool> { Data = true });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assign-staff-role")]
        public async Task<ActionResult<GenericResponse<bool>>> AssignStaffRole([FromBody] AssignStaffRoleDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid input data"));

            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized(new ErrorModel(HttpStatusCode.Unauthorized, "Admin not authenticated"));

            var result = await _authService.AssignStaffRoleAsync(model.UserId, adminId);
            if (!result)
                return StatusCode((int)HttpStatusCode.Forbidden, new ErrorModel(HttpStatusCode.Forbidden, "Unauthorized or user not found"));

            return Ok(new GenericResponse<bool> { Data = true });
        }

        [HttpGet("/me")]
        public async Task<IActionResult> Me()
        {
            var userId = _currentUserService.UserId;

            if (userId == Guid.Empty)
                return Unauthorized(new { message = "User not logged in" });

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                user.Id,
                user.Email,
                user.UserName,
                user.PhoneNumber
                // Add more properties if needed
            });
        }
    }
}