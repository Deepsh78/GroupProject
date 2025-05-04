using AutoMapper;
using GroupApi.Data;
using GroupApi.DTOs.Auth;
using GroupApi.DTOs.Auth.GroupApi.DTOs.Auth;
using GroupApi.Entities.Auth;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GroupApi.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicaionDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;  // Added IConfiguration field

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicaionDbContext context,
            IEmailService emailService,
            IJwtService jwtService,
            IMapper mapper,
            IConfiguration configuration)  // Added IConfiguration to constructor
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailService = emailService;
            _jwtService = jwtService;
            _mapper = mapper;
            _configuration = configuration;  // Initialize the field
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input" });

            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Gender = registerDto.Gender
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });

            var otp = GenerateOtp();
            var otpRecord = new OtpRecord
            {
                UserId = user.Id,
                Code = otp,
                Expiry = DateTime.UtcNow.AddMinutes(10),
                IsForPasswordReset = false
            };
            _context.OtpRecords.Add(otpRecord);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                registerDto.Email,
                "Verify Your Email",
                $"Your OTP is: <b>{otp}</b>. It expires in 10 minutes.");

            return Ok(new { message = "Registration initiated, please verify your email" });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyOtpDto verifyOtpDto)
        {
            var user = await _userManager.FindByEmailAsync(verifyOtpDto.Email);
            if (user == null)
                return BadRequest(new { message = "User not found" });

            var otpRecord = await _context.OtpRecords
                .FirstOrDefaultAsync(o => o.UserId == user.Id && o.Code == verifyOtpDto.Otp && o.Expiry > DateTime.UtcNow && !o.IsForPasswordReset);

            if (otpRecord == null)
                return BadRequest(new { message = "Invalid or expired OTP" });

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            _context.OtpRecords.Remove(otpRecord);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                message = "Email verified successfully",
                token,
                refreshToken,
                user = _mapper.Map<UserProfileDto>(user)
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !user.EmailConfirmed)
                return BadRequest(new { message = "Invalid login attempt or email not verified" });

            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false);
            if (!result.Succeeded)
                return BadRequest(new { message = "Invalid login attempt" });

            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                token,
                refreshToken,
                user = _mapper.Map<UserProfileDto>(user)
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(refreshTokenDto.Token);
            if (principal == null)
                return BadRequest(new { message = "Invalid token" });

            var userId = principal.FindFirst("sub")?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !_jwtService.ValidateRefreshToken(user, refreshTokenDto.RefreshToken))
                return BadRequest(new { message = "Invalid refresh token" });

            var newToken = _jwtService.GenerateToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                token = newToken,
                refreshToken = newRefreshToken,
                user = _mapper.Map<UserProfileDto>(user)
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
                return BadRequest(new { message = "Email not found" });

            var otp = GenerateOtp();
            var otpRecord = new OtpRecord
            {
                UserId = user.Id,
                Code = otp,
                Expiry = DateTime.UtcNow.AddMinutes(10),
                IsForPasswordReset = true
            };
            _context.OtpRecords.Add(otpRecord);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                forgotPasswordDto.Email,
                "Reset Your Password",
                $"Your OTP is: <b>{otp}</b>. It expires in 10 minutes.");

            return Ok(new { message = "OTP sent to your email" });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
        {
            var user = await _userManager.FindByEmailAsync(verifyOtpDto.Email);
            if (user == null)
                return BadRequest(new { message = "User not found" });

            var otpRecord = await _context.OtpRecords
                .FirstOrDefaultAsync(o => o.UserId == user.Id && o.Code == verifyOtpDto.Otp && o.Expiry > DateTime.UtcNow && o.IsForPasswordReset);

            if (otpRecord == null)
                return BadRequest(new { message = "Invalid or expired OTP" });

            return Ok(new { message = "OTP verified, proceed to reset password" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                return BadRequest(new { message = "User not found" });

            var otpRecord = await _context.OtpRecords
                .FirstOrDefaultAsync(o => o.UserId == user.Id && o.Code == resetPasswordDto.Otp && o.Expiry > DateTime.UtcNow && o.IsForPasswordReset);

            if (otpRecord == null)
                return BadRequest(new { message = "Invalid or expired OTP" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });

            _context.OtpRecords.Remove(otpRecord);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password reset successfully" });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (userId == null)
                return Unauthorized(new { message = "User not authenticated" });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest(new { message = "User not found" });

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });

            return Ok(new { message = "Password changed successfully" });
        }

        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = false // Allow expired tokens
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}