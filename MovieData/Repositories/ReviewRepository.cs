using Microsoft.EntityFrameworkCore;

public class ReviewRepository(MovieDbContext context) : IReviewRepository
{
    public async Task<PagedResult<Review>> GetByMovieIdAsync(int movieId, PagingQuery query)
    {
        var reviews = context.Reviews
            .Where(r => r.MovieId == movieId);

        var totalCount = await reviews.CountAsync();

        var items = await reviews
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PagedResult<Review>
        {
            Items = items,
            TotalItems = totalCount,
            CurrentPage = query.Page,
            PageSize = query.PageSize
        };
    }
    public async Task<Review?> GetByIdAsync(int reviewId)
    {
        return await context.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewId);
    }
    public async Task<int> CountByMovieIdAsync(int movieId)
    {
        return await context.Reviews
            .CountAsync(r => r.MovieId == movieId);
    }
    public async Task<int> CountReviewsForMovieAsync(int movieId)
        => await context.Reviews.CountAsync(r => r.MovieId == movieId);

    public async Task<Review?> GetReviewForMovieAsync(int movieId, int reviewId)
        => await context.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.MovieId == movieId);

    public async Task<IEnumerable<Review>> GetAllAsync()
    {
        return await context.Reviews.ToListAsync();
    }
    public async Task<Review?> GetAsync(int id)
    {
        return await context.Reviews.FindAsync(id);
    }

    public void Add(Review review)
    {
        context.Reviews.Add(review);
    }
    public void Remove(Review review)
    {
        context.Reviews.Remove(review);
    }
}
