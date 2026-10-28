using Microsoft.EntityFrameworkCore;
using WordleClone.API.Data;
using WordleClone.API.DTOs;
using WordleClone.API.Models;

namespace WordleClone.API.Services;

public interface ILoginService
{
    Task<LoginResponseDto?> UserAuthentication (string username, string password);
}

public sealed class LoginService : ILoginService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginService(AppDbContext appDbContext, IPasswordHasher passwordHasher,IJwtService jwtService)
    {
        _context = appDbContext;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }
    public async Task<LoginResponseDto?> UserAuthentication(string username, string password)
    {
        User? user = await _context.users.FirstOrDefaultAsync(x => x.Username == username);
        // However, you declared the variable as User, which tells C# that it shouldn't be null when nullable reference types are enabled.
        // Solution is to put ? after User, so that it can contain null or a User object

        if (user == null)
        {
            return null;
        }

        bool isPasswordCorrect = _passwordHasher.VerifyPassword(password, user.PasswordHash);

        if (!isPasswordCorrect)
        {
            return null;
        }
        else
        {
            string token = _jwtService.GenerateToken(user.Id.ToString(), user.Username);

            return new LoginResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Token = token
            };
        }
    }
}