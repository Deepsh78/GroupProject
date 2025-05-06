using GroupApi.DTOs.Auth;
using GroupApi.Entities.Auth;
using GroupApi.Services.CurrentUser;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GroupApi.Controllers.Auth
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
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            return await _authService.RegisterAsync(registerDto);
        }

        [HttpPost("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyOtpDto verifyOtpDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            return await _authService.VerifyEmailAsync(verifyOtpDto);
        }

        [HttpPost("resend-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp([FromBody] ForgotPasswordDto resendOtpDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            return await _authService.ResendOtpAsync(resendOtpDto);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            return await _authService.LoginAsync(loginDto);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            return await _authService.RefreshTokenAsync(refreshTokenDto);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            return await _authService.ForgotPasswordAsync(forgotPasswordDto);
        }

        [HttpPost("verify-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            return await _authService.VerifyOtpAsync(verifyOtpDto);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            return await _authService.ResetPasswordAsync(resetPasswordDto);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            Console.WriteLine($"User authenticated: {User?.Identity?.IsAuthenticated}");
            Console.WriteLine($"Claims: {string.Join(", ", User?.Claims.Select(c => $"{c.Type}: {c.Value}"))}");
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var userId = User.FindFirst("sub")?.Value;
            if (userId == null)
                return Unauthorized(new { message = "User not authenticated" });

            return await _authService.ChangePasswordAsync(userId, changePasswordDto);
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