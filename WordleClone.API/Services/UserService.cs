using WordleClone.API.Data;
using WordleClone.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using WordleClone.API.DTOs;

namespace WordleClone.API.Services;

public interface IUserService
{
    Task<List<UserResponseDto>> GetUsersAsync();
    Task<UserResponseDto?> GetUserAsync(int id);
    Task<User?> CreateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> UpdateUserStatsAsync(int id, int points, int gamesWon);
}

public sealed class UserService : IUserService
{
    /*

    INotification → the last registered implementation.
    IEnumerable<INotification> → all registered implementations.
    
    */
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    public UserService(AppDbContext context, IPasswordHasher passwordHasher) // dependency injection
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }
    public async Task<User?> CreateUserAsync(User user)
    {
        var duplicateUser = await _context.users.FirstOrDefaultAsync(u => u.Username == user.Username);
        if (duplicateUser == null)
        {
            // The database generates identity IDs. Ignore any ID supplied by the client.
            user.Id = 0;
            
            user.PasswordHash = _passwordHasher.HashPassword(user.PasswordHash);

            await _context.users.AddAsync(user);

            await _context.SaveChangesAsync();

            return user;
        }
        else
        {
            return null; // or throw an exception, depending on your design choice
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var userToDelete = await _context.users.FirstOrDefaultAsync(x => x.Id == id);
        if (userToDelete != null)
        {
            _context.users.Remove(userToDelete);
            await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            throw new Exception("No user has this ID (delete)");
        }
    }

    public async Task<UserResponseDto?> GetUserAsync(int id)
    {
        var userToReturn = await _context.users.FirstOrDefaultAsync(x => x.Id == id);
        if (userToReturn == null)
        {
            return null;
        }
        else
        {
            return new UserResponseDto
            {
                Id = userToReturn.Id,
                Username = userToReturn.Username,
                Points = userToReturn.Points,
                GamesWon = userToReturn.GamesWon
            };
        }
    }

    public async Task<List<UserResponseDto>> GetUsersAsync()
    {
        return await _context.users // make a object UserResponseDto of each user, assign each prop, passwordhash is 
        .Select(x => new UserResponseDto
        {
            Id = x.Id,
            Username = x.Username,
            Points = x.Points,
            GamesWon = x.GamesWon
        })
        .ToListAsync();
    }
    public async Task<bool> UpdateUserStatsAsync(int id, int points, int gamesWon)
    {
        var existingUser = await _context.users.FirstOrDefaultAsync(x => x.Id == id);
        if (existingUser == null)
        {
            return false;
        }

        existingUser.Points = points;
        existingUser.GamesWon = gamesWon;
        await _context.SaveChangesAsync();
        return true;
    }
}