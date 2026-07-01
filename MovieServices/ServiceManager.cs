public class ServiceManager(
    IActorService actorService,
    IGenreService genreService,
    IMovieRelationService movieRelationService,
    IMovieService movieService,
    IReviewService reviewService)
    : IServiceManager
{
    public IActorService ActorService { get; } = actorService;
    public IGenreService GenreService { get; } = genreService;
    public IMovieRelationService MovieRelationService { get; } = movieRelationService;
    public IMovieService MovieService { get; } = movieService;
    public IReviewService ReviewService { get; } = reviewService;
}
