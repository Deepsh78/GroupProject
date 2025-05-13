using GroupApi.CommonDomain;
using GroupApi.DTOs;
using GroupApi.DTOs.Reviews;
using GroupApi.Entities;
using GroupApi.Entities.Oders;
using GroupApi.GenericClasses;
using GroupApi.Services.CurrentUser;
using GroupApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GroupApi.Services.Reviews
{
    public class ReviewService : IReviewService
    {
        private readonly IGenericRepository<Review> _reviewRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<OrderItem> _orderItemRepo;

        public ReviewService(
            IGenericRepository<Review> reviewRepo,
            IGenericRepository<Order> orderRepo,
            IGenericRepository<OrderItem> orderItemRepo,
            ICurrentUserService currentUser)
        {
            _reviewRepo = reviewRepo;
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _currentUser = currentUser;
        }

        public async Task<GenericResponse<IEnumerable<ReviewDto>>> GetAllAsync()
        {
            var reviews = await _reviewRepo.TableNoTracking
                .Include(r => r.Book)
                .Include(r => r.Member)
                .ToListAsync();
            var memberId = _currentUser.UserId;

            var result = reviews.Select(r => new ReviewDto
            {
                ReviewId = r.ReviewId,
                BookId = r.BookId,
                Rating = r.Rating,
                Comment = r.Comment,
                MemberId = memberId,
            });

            return result.ToList();
        }

        public async Task<GenericResponse<ReviewDto?>> GetByIdAsync(Guid id)
        {
            var review = await _reviewRepo.TableNoTracking
                .Include(r => r.Book)
                .Include(r => r.Member)
                .FirstOrDefaultAsync(r => r.ReviewId == id);
            var memberId =  _currentUser.UserId;
            if (review == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Review not found");

            return new ReviewDto
            {
                ReviewId = review.ReviewId,
                BookId = review.BookId,
                Rating = review.Rating,
                Comment = review.Comment,
                MemberId = memberId,

            };
        }

        public async Task<GenericResponse<ReviewDto>> CreateAsync(ReviewCreateDto dto)
        {
            var userId = _currentUser.UserId;
            if (userId == Guid.Empty)
                return new ErrorModel(HttpStatusCode.Unauthorized, "User not authenticated");
            var existingReview = await _reviewRepo.TableNoTracking
                .AnyAsync(r => r.BookId == dto.BookId && r.MemberId == userId);

            if (existingReview)
                return new ErrorModel(HttpStatusCode.Conflict, "You have already reviewed this book.");

            var review = new Review
            {
                ReviewId = Guid.NewGuid(),
                MemberId = userId,
                BookId = dto.BookId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };
            if (review.Rating < 1 || review.Rating > 5)
                return new ErrorModel(HttpStatusCode.BadRequest, "Rating must be between 1 and 5.");
            await _reviewRepo.AddAsync(review);
            await _reviewRepo.SaveChangesAsync();

            return new ReviewDto
            {
                ReviewId = review.ReviewId,
                MemberId = review.MemberId,
                BookId = review.BookId,
                Rating = review.Rating,
                Comment = review.Comment
            };
        }


        public async Task<GenericResponse<ReviewDto?>> UpdateAsync(Guid id, UpdateReviewDto dto)
        {
            var userId = _currentUser.UserId;
            if (userId == Guid.Empty)
                return new ErrorModel(HttpStatusCode.Unauthorized, "User not authenticated");

            var review = await _reviewRepo.GetByIdAsync(id);
            if (review == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Review not found");

            if (review.MemberId != userId)
                return new ErrorModel(HttpStatusCode.Forbidden, "You can only update your own reviews.");
            review.BookId = dto.BookId;
            review.Rating = dto.Rating;
            review.Comment = dto.Comment;
            if (review.Rating < 1 || review.Rating > 5)
                return new ErrorModel(HttpStatusCode.BadRequest, "Rating must be between 1 and 5.");
            _reviewRepo.Update(review);
            await _reviewRepo.SaveChangesAsync();

            return new ReviewDto
            {
                ReviewId = review.ReviewId,
                MemberId = userId,
                BookId = review.BookId,
                Rating = review.Rating,
                Comment = review.Comment
            };
        }

        public async Task<Response> DeleteAsync(Guid id)
        {
            var review = await _reviewRepo.GetByIdAsync(id);
            if (review == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Review not found");

            _reviewRepo.Delete(review);
            await _reviewRepo.SaveChangesAsync();

            return new Response();
        }
    }
}
