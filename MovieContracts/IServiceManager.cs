public interface IServiceManager
{
    IActorService ActorService { get; }
    IGenreService GenreService { get; }
    IMovieRelationService MovieRelationService { get; }
    IMovieService MovieService { get; }
    IReviewService ReviewService { get; }
}
