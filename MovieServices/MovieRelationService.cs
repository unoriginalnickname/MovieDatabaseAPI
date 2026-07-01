using AutoMapper;

public class MovieRelationService(IUnitOfWork uow, IMapper mapper) : IMovieRelationService
{
    public async Task<ServiceResult> AddReviewAsync(int movieId, CreateReviewDto dto)
    {
        var movie = await uow.Movies.GetAsync(movieId);

        if (movie == null)
            return ServiceResult.Fail($"No movie with id {movieId}");

        var reviewCount = await uow.Reviews.CountReviewsForMovieAsync(movieId);

        if (reviewCount >= 10)
            return ServiceResult.Fail("A movie can have max 10 reviews");

        var review = mapper.Map<Review>(dto);
        review.MovieId = movieId;

        uow.Reviews.Add(review);
        await uow.CompleteAsync();

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> RemoveReviewAsync(int movieId, int reviewId)
    {
        var review = await uow.Reviews.GetReviewForMovieAsync(movieId, reviewId);

        if (review == null)
            return ServiceResult.Fail("Review not found on this movie");

        uow.Reviews.Remove(review);
        await uow.CompleteAsync();

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> AddActorAsync(int movieId, int actorId)
    {
        var movie = await uow.Movies.GetMovieWithActorsAndGenresAsync(movieId);

        if (movie == null)
            return ServiceResult.Fail($"No movie with id {movieId}");

        var actor = await uow.Actors.GetAsync(actorId);
        if (actor == null)
            return ServiceResult.Fail($"No actor with id {actorId}");

        if (movie.Actors.Any(a => a.Id == actorId))
            return ServiceResult.Fail("Actor already linked to this movie");

        if (IsDocumentaryBudgetViolation(movie, movie.Actors.Count + 1))
            return ServiceResult.Fail("A Documentary film with a budget over 1 million cannot have more than 10 actors.");

        movie.Actors.Add(actor);

        uow.Movies.Update(movie);
        await uow.CompleteAsync();

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> RemoveActorAsync(int movieId, int actorId)
    {
        var movie = await uow.Movies.GetMovieWithActorsAndGenresAsync(movieId);

        if (movie == null)
            return ServiceResult.Fail($"No movie with id {movieId}");

        var actor = movie.Actors.FirstOrDefault(a => a.Id == actorId);
        if (actor == null)
            return ServiceResult.Fail("Actor not linked to this movie");

        movie.Actors.Remove(actor);

        uow.Movies.Update(movie);
        await uow.CompleteAsync();

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> SetActorsAsync(int movieId, List<int> actorIds)
    {
        var movie = await uow.Movies.GetMovieWithActorsAndGenresAsync(movieId);

        if (movie == null)
            return ServiceResult.Fail($"No movie with id {movieId}");

        if (IsDocumentaryBudgetViolation(movie, actorIds.Count))
            return ServiceResult.Fail("A Documentary film with a budget over 1 million cannot have more than 10 actors.");

        movie.Actors = await uow.Actors.GetByIdsAsync(actorIds);

        uow.Movies.Update(movie);
        await uow.CompleteAsync();

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> SetGenresAsync(int movieId, List<int> genreIds)
    {
        var movie = await uow.Movies.GetMovieWithActorsAndGenresAsync(movieId);

        if (movie == null)
            return ServiceResult.Fail($"No movie with id {movieId}");

        movie.Genres = await uow.Genres.GetManyAsync(genreIds);

        uow.Movies.Update(movie);
        await uow.CompleteAsync();

        return ServiceResult.Ok();
    }

    private static bool IsDocumentaryBudgetViolation(Movie movie, int actorCount)
        => movie.Budget > 1_000_000
            && actorCount > 10
            && movie.Genres.Any(g => g.Name == "Documentary");
}