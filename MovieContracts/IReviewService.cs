
public interface IReviewService
{

    Task<PagedResult<ReviewDto>> GetAllAsync(PagingQuery query);
    public Task<ReviewDto?> GetByIdAsync(int reviewId);
    Task<PagedResult<ReviewDto>> GetByMovieIdAsync(
        int movieId,
        PagingQuery query);
    Task<ServiceResult<ReviewDto>> CreateAsync(CreateReviewDto dto);

    Task<ServiceResult> DeleteAsync(int reviewId);
    public Task<ServiceResult> AddReviewAsync(int movieId, CreateReviewDto dto);
}
