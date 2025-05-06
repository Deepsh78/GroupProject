using AutoMapper;
using GroupApi.CommonDomain;
using GroupApi.Data;
using GroupApi.DTOs.Auth;
using GroupApi.Entities.Auth;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace GroupApi.Services.Authorization
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            IEmailService emailService,
            IJwtService jwtService,
            IMapper mapper,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailService = emailService;
            _jwtService = jwtService;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<IActionResult> RegisterAsync(RegisterUserDto registerDto)
        {
            try
            {
                if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
                    return new BadRequestObjectResult(new { message = "Email is already registered" });

                if (await _context.PendingUsers.AnyAsync(p => p.Email == registerDto.Email))
                    return new BadRequestObjectResult(new { message = "Email is pending verification" });

                var tempUser = new ApplicationUser { UserName = registerDto.Email };
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                var passwordHash = passwordHasher.HashPassword(tempUser, registerDto.Password);

                var pendingUser = new PendingUser
                {
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    PasswordHash = passwordHash,
                    Gender = registerDto.Gender,
                    Role = registerDto.Role,
                    CreatedAt = DateTime.UtcNow
                };
                _context.PendingUsers.Add(pendingUser);
                await _context.SaveChangesAsync();

                var otp = GenerateOtp();
                var otpRecord = new OtpRecord
                {
                    UserId = pendingUser.Id.ToString(),
                    Code = otp,
                    Expiry = DateTime.UtcNow.AddMinutes(10),
                    IsForPasswordReset = false
                };
                _context.OtpRecords.Add(otpRecord);
                await _context.SaveChangesAsync();

                try
                {
                    await _emailService.SendEmailAsync(
                        registerDto.Email,
                        "Verify Your Email",
                        $"Your OTP for email verification is: <b>{otp}</b>. It expires in 10 minutes.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send OTP email: {ex.Message}");
                    return new OkObjectResult(new { message = "Registration initiated, but failed to send OTP. Please request a new OTP." });
                }

                return new OkObjectResult(new { message = "Registration initiated, OTP sent to your email" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public async Task<IActionResult> ResendOtpAsync(ForgotPasswordDto resendOtpDto)
        {
            try
            {
                var pendingUser = await _context.PendingUsers
                    .FirstOrDefaultAsync(p => p.Email == resendOtpDto.Email);
                if (pendingUser == null)
                    return new BadRequestObjectResult(new { message = "No pending registration found for this email" });

                var existingOtps = await _context.OtpRecords
                    .Where(o => o.UserId == pendingUser.Id.ToString() && !o.IsForPasswordReset)
                    .ToListAsync();
                _context.OtpRecords.RemoveRange(existingOtps);

                var otp = GenerateOtp();
                var otpRecord = new OtpRecord
                {
                    UserId = pendingUser.Id.ToString(),
                    Code = otp,
                    Expiry = DateTime.UtcNow.AddMinutes(10),
                    IsForPasswordReset = false
                };
                _context.OtpRecords.Add(otpRecord);
                await _context.SaveChangesAsync();

                try
                {
                    await _emailService.SendEmailAsync(
                        resendOtpDto.Email,
                        "Verify Your Email",
                        $"Your new OTP for email verification is: <b>{otp}</b>. It expires in 10 minutes.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send OTP email: {ex.Message}");
                    return new OkObjectResult(new { message = "New OTP generated, but failed to send. Please try again." });
                }

                return new OkObjectResult(new { message = "New OTP sent to your email" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Resend OTP error: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public async Task<IActionResult> VerifyEmailAsync(VerifyOtpDto verifyOtpDto)
        {
            try
            {
                var pendingUser = await _context.PendingUsers
                    .FirstOrDefaultAsync(p => p.Email == verifyOtpDto.Email);
                if (pendingUser == null)
                    return new BadRequestObjectResult(new { message = "User not found" });

                var otpRecord = await _context.OtpRecords
                    .FirstOrDefaultAsync(o => o.UserId == pendingUser.Id.ToString() &&
                                            o.Code == verifyOtpDto.Otp &&
                                            o.Expiry > DateTime.UtcNow &&
                                            !o.IsForPasswordReset);

                if (otpRecord == null)
                    return new BadRequestObjectResult(new { message = "Invalid or expired OTP" });

                var user = new ApplicationUser
                {
                    UserName = pendingUser.Email,
                    Email = pendingUser.Email,
                    FirstName = pendingUser.FirstName,
                    LastName = pendingUser.LastName,
                    Gender = pendingUser.Gender,
                    Role = pendingUser.Role,
                    EmailConfirmed = true
                };

                // Create user with the stored password hash
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return new BadRequestObjectResult(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });

                // Set the password using the stored hash
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = pendingUser.PasswordHash;
                await _userManager.UpdateAsync(user);

                _context.PendingUsers.Remove(pendingUser);
                _context.OtpRecords.Remove(otpRecord);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    message = "Email verified successfully. Please proceed to login.",
                    user = _mapper.Map<UserProfileDto>(user)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email verification error: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public async Task<IActionResult> LoginAsync(LoginUserDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null || !user.EmailConfirmed)
                    return new BadRequestObjectResult(new { message = "Invalid login attempt or email not verified" });

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                if (isPasswordValid == false)
                    return new NotFoundObjectResult(new { message = "Invalid Password!!" });

                var token = _jwtService.GenerateToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);

                return new OkObjectResult(new
                {
                    token,
                    refreshToken,
                    user = _mapper.Map<UserProfileDto>(user)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(refreshTokenDto.Token);
            if (principal == null)
                return new BadRequestObjectResult(new { message = "Invalid token" });

            var userId = principal.FindFirst("sub")?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !_jwtService.ValidateRefreshToken(user, refreshTokenDto.RefreshToken))
                return new BadRequestObjectResult(new { message = "Invalid refresh token" });

            var newToken = _jwtService.GenerateToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new OkObjectResult(new
            {
                token = newToken,
                refreshToken = newRefreshToken,
                user = _mapper.Map<UserProfileDto>(user)
            });
        }

        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
                return new BadRequestObjectResult(new { message = "Email not found" });

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

            try
            {
                await _emailService.SendEmailAsync(
                    forgotPasswordDto.Email,
                    "Reset Your Password",
                    $"Your OTP for password reset is: <b>{otp}</b>. It expires in 10 minutes.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send OTP email: {ex.Message}");
                return new OkObjectResult(new { message = "OTP generated, but failed to send. Please try again." });
            }

            return new OkObjectResult(new { message = "OTP sent to your email" });
        }

        public async Task<IActionResult> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            var user = await _userManager.FindByEmailAsync(verifyOtpDto.Email);
            if (user == null)
                return new BadRequestObjectResult(new { message = "User not found" });

            var otpRecord = await _context.OtpRecords
                .FirstOrDefaultAsync(o => o.UserId == user.Id && o.Code == verifyOtpDto.Otp && o.Expiry > DateTime.UtcNow && o.IsForPasswordReset);

            if (otpRecord == null)
                return new BadRequestObjectResult(new { message = "Invalid or expired OTP" });

            return new OkObjectResult(new { message = "OTP verified, proceed to reset password" });
        }

        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                return new BadRequestObjectResult(new { message = "User not found" });

            var otpRecord = await _context.OtpRecords
                .FirstOrDefaultAsync(o => o.UserId == user.Id && o.Code == resetPasswordDto.Otp && o.Expiry > DateTime.UtcNow && o.IsForPasswordReset);

            if (otpRecord == null)
                return new BadRequestObjectResult(new { message = "Invalid or expired OTP" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
                return new BadRequestObjectResult(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });

            _context.OtpRecords.Remove(otpRecord);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Password reset successfully" });
        }

        public async Task<IActionResult> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new BadRequestObjectResult(new { message = "User not found" });

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded)
                return new BadRequestObjectResult(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });

            return new OkObjectResult(new { message = "Password changed successfully" });
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
                ValidateLifetime = false
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