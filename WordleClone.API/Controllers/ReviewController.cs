using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WordleClone.API.Models;
using WordleClone.API.Services;

[ApiController]
[Route("api/[controller]")]

public sealed class ReviewController: ControllerBase
{
    private readonly IReviewService _reviewService;
    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }
    [HttpGet]
    public async Task<IActionResult> GetReviews()
    {
        var reviews = await _reviewService.GetReviewsAsync();
        return Ok(reviews);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReview(int id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        if (review == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(review);
        }
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview([FromBody] Review review)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized();
        }

        review.UserId = int.Parse(userId);

        await _reviewService.CreateReviewAsync(review);

        return CreatedAtAction(
            nameof(GetReview),
            new { id = review.Id },
            review
        );
    }
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(int id)
    {
        //only the user who created the review can delete it
        var review = await _reviewService.GetReviewByIdAsync(id); 
        if(review == null)
        {
            return NotFound();
        }    
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null || int.Parse(userId) != int.Parse(review.UserId))
        {
            return Forbid();
        }

        await _reviewService.DeleteReviewAsync(id);
        return Ok(true);
    }
}