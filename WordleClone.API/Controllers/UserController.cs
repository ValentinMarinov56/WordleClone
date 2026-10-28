using System.Diagnostics.Contracts;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using WordleClone.API.DTOs;
using WordleClone.API.Models;
using WordleClone.API.Services;

[ApiController]
[Route("api/[controller]")]

public sealed class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    public UserController(IUserService userService, IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetUsersAsync();
        return Ok(users);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _userService.GetUserAsync(id);
        return Ok(user);
    }
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody]User user)
    {
        var createdUser = await _userService.CreateUserAsync(user);

        if (createdUser == null)
        {
            return BadRequest("Username is already taken.");
        }

        string token = _jwtService.GenerateToken(createdUser.Id.ToString(), createdUser.Username);

        return Ok(new LoginResponseDto
        {
            Id = createdUser.Id,
            Username = createdUser.Username,
            Token = token
        });
    }
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateUserStats([FromBody] UpdateUserStatsRequestDto request)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized();

        if (!int.TryParse(userId, out int id))
            return Unauthorized();

        var updated = await _userService.UpdateUserStatsAsync(id, request.Points, request.GamesWon);
        if (!updated)
            return NotFound();

        return Ok(new { request.Points, request.GamesWon });
    }
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var userToDelete = await _userService.GetUserAsync(id);
        if (userToDelete == null)
        {
            return NotFound();
        }
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null && (userId == userToDelete?.Id.ToString()))
        {
            await _userService.DeleteUserAsync(id);
        }
        else
        {
            return Forbid();
        }
        return Ok(true);
    }
}