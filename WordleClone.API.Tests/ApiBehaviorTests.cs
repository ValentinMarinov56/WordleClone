using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using WordleClone.API.DTOs;
using WordleClone.API.Models;
using WordleClone.API.Services;
using Xunit;

namespace WordleClone.API.Tests;

public sealed class ApiBehaviorTests
{
    [Fact]
    public async Task CreateProfile_WhenUsernameIsAvailable_ReturnsProfileAndToken()
    {
        var user = new User { Id = 12, Username = "alice", PasswordHash = "hashed-password" };
        var userService = new FakeUserService { CreatedUser = user };
        var controller = new UserController(userService, new FakeJwtService());

        var result = await controller.CreateUser(new User
        {
            Username = "alice",
            PasswordHash = "password"
        });

        var response = Assert.IsType<LoginResponseDto>(((OkObjectResult)result).Value);
        Assert.Equal(12, response.Id);
        Assert.Equal("alice", response.Username);
        Assert.Equal("test-token", response.Token);
        Assert.Equal("alice", userService.LastCreatedUser?.Username);
    }

    [Fact]
    public async Task CreateProfile_WhenUsernameAlreadyExists_ReturnsBadRequest()
    {
        var userService = new FakeUserService { CreatedUser = null };
        var controller = new UserController(userService, new FakeJwtService());

        var result = await controller.CreateUser(new User
        {
            Username = "alice",
            PasswordHash = "password"
        });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_WhenPasswordIsWrong_ReturnsUnauthorized()
    {
        var controller = new LoginController(new FakeLoginService(null));

        var result = await controller.Authenticate(new LoginRequestDto
        {
            Username = "alice",
            Password = "wrong-password"
        });

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task AddWordl_WhenServiceAcceptsWord_ReturnsSuccess()
    {
        var wordlService = new FakeWordlService { AddResult = true };
        var controller = CreateWordlController(wordlService);

        var result = await controller.CreateWordl(new Wordl { Word = "crane" });

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
        Assert.Equal("crane", wordlService.LastWord?.Word);
    }

    [Fact]
    public async Task AddWordl_WhenWordAlreadyExists_PropagatesDuplicateError()
    {
        var wordlService = new FakeWordlService
        {
            AddException = new InvalidOperationException("This Wordl is already in the database")
        };
        var controller = CreateWordlController(wordlService);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => controller.CreateWordl(new Wordl { Word = "crane" }));

        Assert.Contains("already", exception.Message);
    }

    [Fact]
    public async Task AddWordl_WhenWordIsInvalid_PropagatesValidationError()
    {
        var wordlService = new FakeWordlService
        {
            AddException = new ArgumentException("Word must contain five letters")
        };
        var controller = CreateWordlController(wordlService);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => controller.CreateWordl(new Wordl { Word = "four" }));

        Assert.Contains("five", exception.Message);
    }

    [Theory]
    [InlineData("/api/Wordl")]
    [InlineData("/api/Review")]
    public async Task ProtectedPost_WhenTokenIsExpired_ReturnsUnauthorized(string path)
    {
        using var app = await CreateTestServerAsync();
        using var client = app.GetTestClient();
        var expiredToken = CreateExpiredToken();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", expiredToken);

        var response = await client.PostAsync(path, new StringContent("{}", Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static WordlController CreateWordlController(IWordlService service)
    {
        var controller = new WordlController(service);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, "42") }, "Test"))
            }
        };
        return controller;
    }

    private static async Task<WebApplication> CreateTestServerAsync()
    {
        const string testSecret = "unit-test-signing-key-must-be-at-least-32-bytes";
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(testSecret));

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddControllers().AddApplicationPart(typeof(WordlController).Assembly);
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        builder.Services.AddAuthorization();
        builder.Services.AddSingleton<IWordlService>(new FakeWordlService());
        builder.Services.AddSingleton<IReviewService>(new FakeReviewService());

        var app = builder.Build();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        await app.StartAsync();
        return app;
    }

    private static string CreateExpiredToken()
    {
        const string testSecret = "unit-test-signing-key-must-be-at-least-32-bytes";
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(testSecret));
        var token = new JwtSecurityToken(
            claims: new[] { new Claim(ClaimTypes.NameIdentifier, "42") },
            notBefore: DateTime.UtcNow.AddMinutes(-10),
            expires: DateTime.UtcNow.AddMinutes(-5),
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private sealed class FakeUserService : IUserService
    {
        public User? CreatedUser { get; set; }
        public User? LastCreatedUser { get; private set; }

        public Task<User?> CreateUserAsync(User user)
        {
            LastCreatedUser = user;
            return Task.FromResult(CreatedUser);
        }

        public Task<bool> DeleteUserAsync(int id) => Task.FromResult(true);
        public Task<UserResponseDto?> GetUserAsync(int id) => Task.FromResult<UserResponseDto?>(null);
        public Task<List<UserResponseDto>> GetUsersAsync() => Task.FromResult(new List<UserResponseDto>());
        public Task<bool> UpdateUserStatsAsync(int id, int points, int gamesWon) => Task.FromResult(true);
    }

    private sealed class FakeJwtService : IJwtService
    {
        public string GenerateToken(string userId, string Username) => "test-token";
    }

    private sealed class FakeLoginService(LoginResponseDto? response) : ILoginService
    {
        public Task<LoginResponseDto?> UserAuthentication(string username, string password) => Task.FromResult(response);
    }

    private sealed class FakeWordlService : IWordlService
    {
        public bool AddResult { get; init; }
        public Exception? AddException { get; init; }
        public Wordl? LastWord { get; private set; }

        public Task<bool> AddWordl(Wordl? newWordl)
        {
            LastWord = newWordl;
            if (AddException is not null) throw AddException;
            return Task.FromResult(AddResult);
        }

        public Task<List<Wordl>> GetAllWordl() => Task.FromResult(new List<Wordl>());
        public Task<Wordl?> GetWordl(int id) => Task.FromResult<Wordl?>(null);
        public Task<bool> RemoveWordl(int id) => Task.FromResult(true);
    }

    private sealed class FakeReviewService : IReviewService
    {
        public Task<Review> CreateReviewAsync(Review review) => Task.FromResult(review);
        public Task<bool> DeleteReviewAsync(int id) => Task.FromResult(true);
        public Task<ReviewResponseDto?> GetReviewByIdAsync(int id) => Task.FromResult<ReviewResponseDto?>(null);
        public Task<List<ReviewResponseDto>> GetReviewsAsync() => Task.FromResult(new List<ReviewResponseDto>());
    }
}
