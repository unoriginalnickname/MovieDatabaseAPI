public interface IMovieRepository
{
    IQueryable<Movie> Query();

    public Task<Movie?> GetWithDetailsAsync(int id);
    Task<Movie?> GetByIdAsync(int id);

    Task<bool> TitleExistsAsync(string title, int? excludeId = null);

    Task<PagedResult<Movie>> SearchAsync(MovieQuery query);

    void Add(Movie movie);

    void Remove(Movie movie);
}