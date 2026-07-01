using AutoMapper;

public class ReviewService(IUnitOfWork uow, IMapper mapper) : IReviewService
{
    public async Task<ServiceResult> AddReviewAsync(int movieId, CreateReviewDto dto)
    {
        var movie = await uow.Movies.GetAsync(movieId);

        if (movie == null)
            return ServiceResult.Fail("Movie not found");

        var reviewCount = await uow.Reviews.CountByMovieIdAsync(movieId);

        if (reviewCount >= 10)
            return ServiceResult.Fail("A movie can have max 10 reviews");

        var review = mapper.Map<Review>(dto);
        review.MovieId = movieId;

        uow.Reviews.Add(review);
        await uow.CompleteAsync();

        return ServiceResult.Ok();
    }

    public async Task<PagedResult<ReviewDto>> GetAllAsync(PagingQuery query)
    {
        var result = await uow.Reviews.GetAllAsync();

        return new PagedResult<ReviewDto>
        {
            Items = mapper.Map<List<ReviewDto>>(result),
            TotalItems = result.Count(),
            CurrentPage = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<PagedResult<ReviewDto>> GetByMovieIdAsync(int movieId, PagingQuery query)
    {
        var result = await uow.Reviews.GetByMovieIdAsync(movieId, query);

        return new PagedResult<ReviewDto>
        {
            Items = mapper.Map<List<ReviewDto>>(result.Items),
            TotalItems = result.TotalItems,
            CurrentPage = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<ReviewDto?> GetByIdAsync(int reviewId)
    {
        var review = await uow.Reviews.GetByIdAsync(reviewId);
        return review == null ? null : mapper.Map<ReviewDto>(review);
    }

    public async Task<ServiceResult<ReviewDto>> CreateAsync(CreateReviewDto dto)
    {
        var review = mapper.Map<Review>(dto);

        uow.Reviews.Add(review);
        await uow.CompleteAsync();

        return ServiceResult<ReviewDto>.Ok(mapper.Map<ReviewDto>(review));
    }

    public async Task<ServiceResult> DeleteAsync(int reviewId)
    {
        var review = await uow.Reviews.GetAsync(reviewId);

        if (review == null)
            return ServiceResult.Fail("Review not found");

        uow.Reviews.Remove(review);
        await uow.CompleteAsync();

        return ServiceResult.Ok();
    }
}