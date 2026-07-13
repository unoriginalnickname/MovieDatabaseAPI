public interface IReviewService
{
    Task<ServiceResult<PagedResult<ReviewDto>>> GetAllAsync(PagingQuery query);

    Task<ServiceResult<ReviewDto>> GetByIdAsync(int reviewId);

    Task<ServiceResult<PagedResult<ReviewDto>>> GetByMovieIdAsync(
        int movieId,
        PagingQuery query);

    Task<ServiceResult> DeleteAsync(int reviewId);
}