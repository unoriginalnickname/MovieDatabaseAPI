public class UnitOfWork(MovieDbContext context) : IUnitOfWork
{
    public IMovieRepository Movies { get; } = new MovieRepository(context);
    public IReviewRepository Reviews { get; } = new ReviewRepository(context);
    public IActorRepository Actors { get; } = new ActorRepository(context);
    public IGenreRepository Genres { get; } = new GenreRepository(context);

    public async Task CompleteAsync() => await context.SaveChangesAsync();
}
