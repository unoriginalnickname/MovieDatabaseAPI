public interface IMovieRepository
{
    public Task<Movie?> GetMovieWithActorsAndGenresAsync(int movieId);
    Task<Movie?> GetAsync(int id);

    Task<Movie?> GetWithDetailsAsync(int id);

    Task<Movie?> GetForPatchAsync(int id);

    Task<bool> TitleExistsAsync(string title, int? excludeId = null);

    Task<PagedResult<Movie>> SearchAsync(MovieQuery query);

    void Add(Movie movie);

    void Update(Movie movie);

    void Remove(Movie movie);
}