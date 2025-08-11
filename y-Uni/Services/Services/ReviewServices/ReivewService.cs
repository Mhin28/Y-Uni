using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.ResultModels;
using Repositories.ViewModels.ReviewModel;
using Services.Services.ReviewServices;

public class ReviewService : IReviewService
{
    private readonly IReviewRepo _reviewRepository;

    public ReviewService(IReviewRepo reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<ResultModel> GetAllAsync()
    {
        var result = new ResultModel();
        try
        {
            var reviews = await _reviewRepository.GetAllAsync();
            result.IsSuccess = true;
            result.Data = reviews;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Error: {ex.Message}";
        }
        return result;
    }

    public async Task<ResultModel> GetByUserIdAsync(Guid userId)
    {
        var result = new ResultModel();
        try
        {
            var review = await _reviewRepository.GetByUserIdAsync(userId);
            if (review != null)
            {
                result.IsSuccess = true;
                result.Data = review;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Review not found for this user.";
            }
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Error: {ex.Message}";
        }
        return result;
    }

    public async Task<ResultModel> CreateOrUpdateAsync(Guid userId, int? rating, string comment)
    {
        var result = new ResultModel();
        try
        {
            var review = await _reviewRepository.GetByUserIdAsync(userId);

            if (review == null)
            {
                review = new Review
                {
                    ReviewId = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _reviewRepository.AddAsync(review);
            }

            if (rating.HasValue)
                review.Rating = rating.Value;

            if (!string.IsNullOrWhiteSpace(comment))
                review.Comment = comment;

            review.UpdatedAt = DateTime.UtcNow;

            await _reviewRepository.SaveChangesAsync();

            result.IsSuccess = true;
            result.Message = review.CreatedAt == review.UpdatedAt
                ? "Review created successfully."
                : "Review updated successfully.";
            result.Data = review;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Error: {ex.Message}";
        }
        return result;
    }


    public async Task<ResultModel> DeleteAsync(Guid reviewId)
    {
        var result = new ResultModel();
        try
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review != null)
            {
                await _reviewRepository.DeleteAsync(review);
                result.IsSuccess = true;
                result.Message = "Review deleted successfully.";
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Review not found.";
            }
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Error: {ex.Message}";
        }
        return result;
    }
}
