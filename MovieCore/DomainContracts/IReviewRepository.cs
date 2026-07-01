public interface IReviewRepository
{
    public Task<PagedResult<Review>> GetByMovieIdAsync(int movieId, PagingQuery query);
    public Task<Review?> GetByIdAsync(int reviewId);
    public Task<int> CountByMovieIdAsync(int movieId);
    public Task<int> CountReviewsForMovieAsync(int movieId);
    public Task<Review?> GetReviewForMovieAsync(int movieId, int reviewId);
    Task<IEnumerable<Review>> GetAllAsync();
    Task<Review?> GetAsync(int id);
    void Add(Review review);
    void Remove(Review review);
}
