using Microsoft.EntityFrameworkCore;
using WordleClone.API.Data;
using WordleClone.API.DTOs;
using WordleClone.API.Models;

namespace WordleClone.API.Services;

public interface IReviewService
{
    Task<List<ReviewResponseDto>> GetReviewsAsync();
    Task<ReviewResponseDto?> GetReviewByIdAsync(int id);
    Task<Review> CreateReviewAsync(Review review);
    Task<bool> DeleteReviewAsync(int id);
}

public sealed class ReviewService : IReviewService
{
    public readonly AppDbContext _context;
    public ReviewService(AppDbContext context)
    {
        _context = context;
    }  
    public async Task<Review> CreateReviewAsync(Review review)
    {
        await _context.reviews.AddAsync(review);
        await _context.SaveChangesAsync();
        return review;
    }

    public async Task<bool> DeleteReviewAsync(int id)
    {
        Review? reviewToDelete = await _context.reviews.FirstOrDefaultAsync(x => x.Id == id);
        if (reviewToDelete == null)
        {
            throw new Exception("Review doesnt exist");
        }
        else
        {
            _context.reviews.Remove(reviewToDelete);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public async Task<ReviewResponseDto?> GetReviewByIdAsync(int id)
    {
        Review? reviewToReturn = await _context.reviews.FirstOrDefaultAsync(x => x.Id == id);
        if (reviewToReturn == null)
        {
            return null;
        }
        else
        {
            return new ReviewResponseDto
            {
                Id = reviewToReturn.Id,
                Text = reviewToReturn.Text,
                AuthorUsername = reviewToReturn.User?.Username ?? "unknown",
                UserId = reviewToReturn.UserId?.ToString() ?? "0" //used to be reviewToReturn.User?.Id.toString() but we didnt include the user in the review so we cant access it like that anymore
                // first questionmark makes the userId be able to be null while the 2nd and 3rd one mean if its null to replace it with "unknown"
            };
        }
    }

    public async Task<List<ReviewResponseDto>> GetReviewsAsync()
    {
        return await _context.reviews.Select(x => new ReviewResponseDto // go though every review
                            {
                                Id = x.Id,
                                Text = x.Text,
                                AuthorUsername = x.User == null ? "unkown" : x.User.Username, // if the user is null do elvis if
                                UserId = x.UserId == null ? "0" : x.UserId.Value.ToString()
                            })
                            .ToListAsync();
    }
}