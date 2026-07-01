using AutoMapper;
using Microsoft.Extensions.Logging;

public class ReviewService(
    IUnitOfWork uow,
    IMapper mapper,
    ILogger<ReviewService> logger) : IReviewService
{
    public async Task<ServiceResult> AddReviewAsync(int movieId, CreateReviewDto dto)
    {
        var movie = await uow.Movies.GetByIdAsync(movieId);

        if (movie == null)
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.NotFound,
                "Movie not found",
                nameof(AddReviewAsync));

        var reviewCount = await uow.Reviews.CountByMovieIdAsync(movieId);

        if (reviewCount >= 10)
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.Validation,
                "A movie can have max 10 reviews",
                nameof(AddReviewAsync));

        var review = mapper.Map<Review>(dto);
        review.MovieId = movieId;

        uow.Reviews.Add(review);
        await uow.CompleteAsync();

        return ServiceResultFactory.Ok();
    }

    public async Task<ServiceResult<PagedResult<ReviewDto>>> GetAllAsync(PagingQuery query)
    {
        var result = await uow.Reviews.GetAllAsync();

        var mapped = new PagedResult<ReviewDto>
        {
            Items = mapper.Map<List<ReviewDto>>(result),
            TotalItems = result.Count(),
            CurrentPage = query.Page,
            PageSize = query.PageSize
        };

        return ServiceResultFactory.Ok(mapped);
    }

    public async Task<ServiceResult<PagedResult<ReviewDto>>> GetByMovieIdAsync(int movieId, PagingQuery query)
    {
        var movie = await uow.Movies.GetByIdAsync(movieId);

        if (movie == null)
            return ServiceResultFactory.Fail<PagedResult<ReviewDto>>(
                logger,
                ErrorTypeEnum.NotFound,
                "Movie not found",
                nameof(GetByMovieIdAsync));

        var result = await uow.Reviews.GetByMovieIdAsync(movieId, query);

        var mapped = new PagedResult<ReviewDto>
        {
            Items = mapper.Map<List<ReviewDto>>(result.Items),
            TotalItems = result.TotalItems,
            CurrentPage = query.Page,
            PageSize = query.PageSize
        };

        return ServiceResultFactory.Ok(mapped);
    }

    public async Task<ServiceResult<ReviewDto>> GetByIdAsync(int reviewId)
    {
        var review = await uow.Reviews.GetByIdAsync(reviewId);

        if (review == null)
            return ServiceResultFactory.Fail<ReviewDto>(
                logger,
                ErrorTypeEnum.NotFound,
                "Review not found",
                nameof(GetByIdAsync));

        return ServiceResultFactory.Ok(mapper.Map<ReviewDto>(review));
    }

    public async Task<ServiceResult<ReviewDto>> CreateAsync(CreateReviewDto dto)
    {
        var review = mapper.Map<Review>(dto);

        uow.Reviews.Add(review);
        await uow.CompleteAsync();

        return ServiceResultFactory.Ok(mapper.Map<ReviewDto>(review));
    }

    public async Task<ServiceResult> DeleteAsync(int reviewId)
    {
        var review = await uow.Reviews.GetAsync(reviewId);

        if (review == null)
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.NotFound,
                "Review not found",
                nameof(DeleteAsync));

        uow.Reviews.Remove(review);
        await uow.CompleteAsync();

        return ServiceResultFactory.Ok();
    }
}