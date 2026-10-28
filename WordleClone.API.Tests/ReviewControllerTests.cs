using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WordleClone.API.DTOs;
using WordleClone.API.Models;
using WordleClone.API.Services;
using Xunit;

namespace WordleClone.API.Tests;

public sealed class ReviewControllerTests
{
    [Fact]
    public async Task DeleteReview_WhenAuthorDeletesReview_ReturnsOkAndDeletes()
    {
        var reviewService = new FakeReviewService(new ReviewResponseDto
        {
            Id = 7,
            Text = "Great game",
            AuthorUsername = "alice",
            UserId = "42"
        });
        var controller = CreateController(reviewService, "42");

        var result = await controller.DeleteReview(7);

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(7, reviewService.DeletedReviewId);
    }

    [Fact]
    public async Task DeleteReview_WhenDifferentUserRequestsDelete_ReturnsForbid()
    {
        var reviewService = new FakeReviewService(new ReviewResponseDto
        {
            Id = 7,
            Text = "Great game",
            AuthorUsername = "alice",
            UserId = "42"
        });
        var controller = CreateController(reviewService, "99");

        var result = await controller.DeleteReview(7);

        Assert.IsType<ForbidResult>(result);
        Assert.Null(reviewService.DeletedReviewId);
    }

    [Fact]
    public async Task DeleteReview_WhenReviewDoesNotExist_ReturnsNotFound()
    {
        var reviewService = new FakeReviewService(null);
        var controller = CreateController(reviewService, "42");

        var result = await controller.DeleteReview(7);

        Assert.IsType<NotFoundResult>(result);
        Assert.Null(reviewService.DeletedReviewId);
    }

    private static ReviewController CreateController(FakeReviewService service, string userId)
    {
        var controller = new ReviewController(service);
        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, userId) },
            authenticationType: "Test");
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };
        return controller;
    }

    private sealed class FakeReviewService(ReviewResponseDto? review) : IReviewService
    {
        public int? DeletedReviewId { get; private set; }

        public Task<List<ReviewResponseDto>> GetReviewsAsync() => Task.FromResult(new List<ReviewResponseDto>());

        public Task<ReviewResponseDto?> GetReviewByIdAsync(int id) => Task.FromResult(review);

        public Task<Review> CreateReviewAsync(Review value) => Task.FromResult(value);

        public Task<bool> DeleteReviewAsync(int id)
        {
            DeletedReviewId = id;
            return Task.FromResult(true);
        }
    }
}
