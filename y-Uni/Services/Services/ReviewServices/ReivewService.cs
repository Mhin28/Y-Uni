using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.ResultModels;
using Services.Services.ReviewServices;
using Services.Services.TokenService;
using System.Net;

public class ReviewService : IReviewService
{
    private readonly IReviewRepo _reviewRepo;
    private readonly ITokenService _tokenService;

    public ReviewService(IReviewRepo reviewRepo, ITokenService tokenService)
    {
        _reviewRepo = reviewRepo;
        _tokenService = tokenService;
    }

    public async Task<ResultModel> GetAllReviewsAsync()
    {
        var result = new ResultModel
        {
            IsSuccess = false,
            Code = (int)HttpStatusCode.NoContent,
            Message = "No reviews found."
        };

        var reviews = await _reviewRepo.GetAllAsync();
        if (reviews == null || !reviews.Any())
        {
            return result;
        }

        result.IsSuccess = true;
        result.Code = (int)HttpStatusCode.OK;
        result.Message = "Reviews retrieved successfully.";
        result.Data = reviews;
        return result;
    }

    public async Task<ResultModel> GetReviewsByUserIdAsync(Guid userId)
    {
        var result = new ResultModel
        {
            IsSuccess = false,
            Code = (int)HttpStatusCode.NoContent,
            Message = "No reviews found for this user."
        };

        var reviews = await _reviewRepo.GetByUserIdAsync(userId);
        if (reviews == null || !reviews.Any())
        {
            return result;
        }

        result.IsSuccess = true;
        result.Code = (int)HttpStatusCode.OK;
        result.Message = "User reviews retrieved successfully.";
        result.Data = reviews;
        return result;
    }

    public async Task<ResultModel> CreateReviewAsync(string token, int rating, string comment)
    {
        var result = new ResultModel
        {
            IsSuccess = false,
            Code = (int)HttpStatusCode.BadRequest,
            Message = "Failed to create review."
        };

        if (string.IsNullOrEmpty(token))
        {
            result.Code = (int)HttpStatusCode.Unauthorized;
            result.Message = "Invalid token.";
            return result;
        }

        var decoded = _tokenService.decode(token);
        if (decoded == null || string.IsNullOrEmpty(decoded.userid) || !Guid.TryParse(decoded.userid, out Guid userId))
        {
            result.Code = (int)HttpStatusCode.Unauthorized;
            result.Message = "Invalid token.";
            return result;
        }

        if (rating < 1 || rating > 5)
        {
            result.Code = (int)HttpStatusCode.BadRequest;
            result.Message = "Rating must be between 1 and 5.";
            return result;
        }

        var review = new Review
        {
            ReviewId = Guid.NewGuid(),
            UserId = userId,
            Rating = rating,
            Comment = comment,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _reviewRepo.CreateAsync(review);

        result.IsSuccess = true;
        result.Code = (int)HttpStatusCode.Created;
        result.Message = "Review created successfully.";
        result.Data = created;
        return result;
    }

   
    public async Task<ResultModel> UpdateReviewAsync(Guid userId, Guid reviewId, int rating, string comment)
    {
        var result = new ResultModel
        {
            IsSuccess = false,
            Code = (int)HttpStatusCode.BadRequest,
            Message = "Failed to update review."
        };

        var existingReview = await _reviewRepo.GetByIdAsync(reviewId);
        if (existingReview == null)
        {
            result.Code = (int)HttpStatusCode.NotFound;
            result.Message = "Review not found.";
            return result;
        }

        if (existingReview.UserId != userId)
        {
            result.Code = (int)HttpStatusCode.Forbidden;
            result.Message = "You do not have permission to update this review.";
            return result;
        }

        if (rating < 1 || rating > 5)
        {
            result.Message = "Rating must be between 1 and 5.";
            return result;
        }

        existingReview.Rating = rating;
        existingReview.Comment = comment;
        existingReview.UpdatedAt = DateTime.UtcNow;

        var updated = await _reviewRepo.UpdateAsync(existingReview);

        result.IsSuccess = true;
        result.Code = (int)HttpStatusCode.OK;
        result.Message = "Review updated successfully.";
        result.Data = updated;

        return result;
    }


}
