using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GroupApi.Constants;
using GroupApi.Constraint;
using GroupApi.Data;
using GroupApi.DTOs.Auth;
using GroupApi.Entities.Auth;
using GroupApi.Services.Interface;
using GroupApi.Entities;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailService _emailService;
    private readonly IJwtService _jwtService;
    private readonly ApplicationDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailService emailService,
        IJwtService jwtService,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _jwtService = jwtService;
        _context = context;
    }
    public async Task<bool> CreateAdminAsync(RegisterDto model)
    {
        // Check if email already exists
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
            return false;

        // Create new admin user
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Gender = model.Gender,
            Role = RoleType.Admin
        };

        // Create user with password
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return false;

        // Create corresponding Member entity
        var member = new Member
        {
            MemberId = Guid.Parse(user.Id),
            UserName = user.UserName,
            Email = user.Email,
            Password = user.PasswordHash,
            OrderCount = 0
        };

        await _context.Members.AddAsync(member);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RegisterAsync(RegisterDto model)
    {
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
            return false;

        var tempUser = new TempUserRegistration
        {
            Id = Guid.NewGuid(),
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, model.Password),
            Gender = model.Gender,
            OTP = GenerateOTP(),
            OTPExpiration = DateTime.UtcNow.AddMinutes(10),
            CreatedAt = DateTime.UtcNow
        };

        await _context.TempUserRegistrations.AddAsync(tempUser);
        await _context.SaveChangesAsync();

        await _emailService.SendOtpEmailAsync(model.Email, tempUser.OTP, "registration");
        return true;
    }

    public async Task<bool> VerifyOtpAsync(VerifyOtpDto model)
    {
        var tempUser = await _context.TempUserRegistrations
            .FirstOrDefaultAsync(t => t.Email == model.Email && t.OTP == model.OTP);

        if (tempUser == null || tempUser.OTPExpiration < DateTime.UtcNow)
            return false;

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(), // Ensure a unique ID for ApplicationUser
            UserName = tempUser.Email,
            Email = tempUser.Email,
            FirstName = tempUser.FirstName,
            LastName = tempUser.LastName,
            Gender = tempUser.Gender,
            Role = RoleType.User
        };

        // Create user without setting password initially
        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
            return false;

        // Set the PasswordHash directly from TempUserRegistration
        user.PasswordHash = tempUser.PasswordHash;
        await _userManager.UpdateAsync(user);

        // Create corresponding Member entity
        var member = new Member
        {
            MemberId = Guid.Parse(user.Id), // Use the same ID as ApplicationUser
            UserName = user.UserName,
            Email = user.Email,
            Password = user.PasswordHash, // Store the hashed password
            OrderCount = 0 // Initialize OrderCount
        };

        await _context.Members.AddAsync(member);

        // Clean up TempUserRegistration and save all changes
        _context.TempUserRegistrations.Remove(tempUser);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<string> LoginAsync(LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return null;

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
        if (!result.Succeeded)
            return null;

        return await _jwtService.GenerateJwtTokenAsync(user);
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return false;

        var tempReset = new TempPasswordReset
        {
            Id = Guid.NewGuid(),
            Email = model.Email,
            OTP = GenerateOTP(),
            OTPExpiration = DateTime.UtcNow.AddMinutes(10),
            CreatedAt = DateTime.UtcNow
        };

        await _context.TempPasswordResets.AddAsync(tempReset);
        await _context.SaveChangesAsync();

        await _emailService.SendOtpEmailAsync(model.Email, tempReset.OTP, "password reset");
        return true;
    }

    public async Task<bool> VerifyPasswordResetOtpAsync(VerifyOtpDto model)
    {
        var tempReset = await _context.TempPasswordResets
            .FirstOrDefaultAsync(t => t.Email == model.Email && t.OTP == model.OTP);

        if (tempReset == null || tempReset.OTPExpiration < DateTime.UtcNow)
            return false;

        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return false;

        var tempReset = await _context.TempPasswordResets
            .FirstOrDefaultAsync(t => t.Email == model.Email && t.OTPExpiration > DateTime.UtcNow);

        if (tempReset == null)
            return false;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

        if (result.Succeeded)
        {
            _context.TempPasswordResets.Remove(tempReset);
            await _context.SaveChangesAsync();
        }

        return result.Succeeded;
    }

    public async Task<bool> AssignStaffRoleAsync(string userId)
    {
       

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        user.Role = RoleType.Staff;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
    public async Task<bool> AssignAdminRoleAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        user.Role = RoleType.Admin;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> ResendOtpAsync(ResendOtpDto model)
    {
        if (model.Purpose != "registration" && model.Purpose != "password reset")
            return false;

        if (model.Purpose == "registration")
        {
            var tempUser = await _context.TempUserRegistrations
                .FirstOrDefaultAsync(t => t.Email == model.Email);

            if (tempUser == null)
                return false;

            tempUser.OTP = GenerateOTP();
            tempUser.OTPExpiration = DateTime.UtcNow.AddMinutes(10);
            await _context.SaveChangesAsync();

            await _emailService.SendOtpEmailAsync(model.Email, tempUser.OTP, "registration");
            return true;
        }
        else // password reset
        {
            var tempReset = await _context.TempPasswordResets
                .FirstOrDefaultAsync(t => t.Email == model.Email);

            if (tempReset == null)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return false;

                tempReset = new TempPasswordReset
                {
                    Id = Guid.NewGuid(),
                    Email = model.Email,
                    OTP = GenerateOTP(),
                    OTPExpiration = DateTime.UtcNow.AddMinutes(10),
                    CreatedAt = DateTime.UtcNow
                };

                await _context.TempPasswordResets.AddAsync(tempReset);
            }
            else
            {
                tempReset.OTP = GenerateOTP();
                tempReset.OTPExpiration = DateTime.UtcNow.AddMinutes(10);
            }

            await _context.SaveChangesAsync();
            await _emailService.SendOtpEmailAsync(model.Email, tempReset.OTP, "password reset");
            return true;
        }
    }
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Gender = u.Gender,
                Role = u.Role
            })
            .ToListAsync();

        return users;
    }

    private string GenerateOTP()
    {
        byte[] randomBytes = new byte[4];
        RandomNumberGenerator.Fill(randomBytes);
        int number = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % 900000 + 100000; // 100000-999999
        return number.ToString();
    }
}