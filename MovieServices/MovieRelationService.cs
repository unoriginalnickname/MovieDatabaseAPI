using AutoMapper;
using Microsoft.Extensions.Logging;

public class MovieRelationService(IUnitOfWork uow, IMapper mapper, ILogger<MovieRelationService> logger)
    : IMovieRelationService
{
    public async Task<ServiceResult> AddReviewAsync(int movieId, CreateReviewDto dto)
    {
        var movie = await uow.Movies.GetByIdAsync(movieId);

        if (movie == null)
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.NotFound,
                $"No movie with id {movieId}",
                nameof(AddReviewAsync));

        var reviewCount = await uow.Reviews.CountReviewsForMovieAsync(movieId);

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

    public async Task<ServiceResult> RemoveReviewAsync(int movieId, int reviewId)
    {
        var review = await uow.Reviews.GetReviewForMovieAsync(movieId, reviewId);

        if (review == null)
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.NotFound,
                "Review not found on this movie",
                nameof(RemoveReviewAsync));

        uow.Reviews.Remove(review);
        await uow.CompleteAsync();

        return ServiceResultFactory.Ok();
    }

    public async Task<ServiceResult> AddActorAsync(int movieId, int actorId)
    {
        var movie = await uow.Movies.GetByIdAsync(movieId);

        if (movie == null)
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.NotFound,
                $"No movie with id {movieId}",
                nameof(AddActorAsync));

        var actor = await uow.Actors.GetAsync(actorId);

        if (actor == null)
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.NotFound,
                $"No actor with id {actorId}",
                nameof(AddActorAsync));

        if (movie.Actors.Any(a => a.Id == actorId))
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.Conflict,
                "Actor already linked to this movie",
                nameof(AddActorAsync));

        if (IsDocumentaryBudgetViolation(movie, movie.Actors.Count + 1))
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.Validation,
                "A Documentary film with a budget over 1 million cannot have more than 10 actors.",
                nameof(AddActorAsync));

        movie.Actors.Add(actor);

        await uow.CompleteAsync();

        return ServiceResultFactory.Ok();
    }

    public async Task<ServiceResult> RemoveActorAsync(int movieId, int actorId)
    {
        var movie = await uow.Movies.GetWithDetailsAsync(movieId);

        if (movie == null)
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.NotFound,
                $"No movie with id {movieId}",
                nameof(RemoveActorAsync));

        var actor = movie.Actors.FirstOrDefault(a => a.Id == actorId);

        if (actor == null)
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.NotFound,
                "Actor not linked to this movie",
                nameof(RemoveActorAsync));

        movie.Actors.Remove(actor);

        await uow.CompleteAsync();

        return ServiceResultFactory.Ok();
    }

    public async Task<ServiceResult> SetActorsAsync(int movieId, List<int> actorIds)
    {
        var movie = await uow.Movies.GetWithDetailsAsync(movieId);

        if (movie == null)
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.NotFound,
                $"No movie with id {movieId}",
                nameof(SetActorsAsync));

        if (IsDocumentaryBudgetViolation(movie, actorIds.Count))
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.Validation,
                "A Documentary film with a budget over 1 million cannot have more than 10 actors.",
                nameof(SetActorsAsync));

        movie.Actors = await uow.Actors.GetByIdsAsync(actorIds);

        await uow.CompleteAsync();

        return ServiceResultFactory.Ok();
    }

    public async Task<ServiceResult> SetGenresAsync(int movieId, List<int> genreIds)
    {
        var movie = await uow.Movies.GetWithDetailsAsync(movieId);

        if (movie == null)
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.NotFound,
                $"No movie with id {movieId}",
                nameof(SetGenresAsync));

        movie.Genres = await uow.Genres.GetManyAsync(genreIds);

        await uow.CompleteAsync();

        return ServiceResultFactory.Ok();
    }

    private static bool IsDocumentaryBudgetViolation(Movie movie, int actorCount)
        => movie.Budget > 1_000_000
           && actorCount > 10
           && movie.Genres.Any(g => g.Name == "Documentary");
}