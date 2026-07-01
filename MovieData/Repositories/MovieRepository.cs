using Microsoft.EntityFrameworkCore;

public class MovieRepository(MovieDbContext context) : IMovieRepository
{
    public IQueryable<Movie> Query()
        => context.Movies;
    public Task<Movie?> GetByIdAsync(int id)
        => context.Movies.FirstOrDefaultAsync(x => x.Id == id);

    public Task<Movie?> GetWithDetailsAsync(int id)
        => context.Movies
            .Include(x => x.Actors)
            .Include(x => x.Genres)
            .FirstOrDefaultAsync(x => x.Id == id);

    public Task<bool> TitleExistsAsync(string title, int? excludeId = null)
        => context.Movies.AnyAsync(m =>
            m.Title == title &&
            (!excludeId.HasValue || m.Id != excludeId));

    public async Task<PagedResult<Movie>> SearchAsync(MovieQuery query)
    {
        var q = context.Movies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Title))
            q = q.Where(m => m.Title.Contains(query.Title));

        if (query.Year.HasValue)
            q = q.Where(m => m.Year == query.Year);

        if (!string.IsNullOrWhiteSpace(query.Language))
            q = q.Where(m => m.Language == query.Language);

        if (!string.IsNullOrWhiteSpace(query.Genre))
            q = q.Where(m => m.Genres.Any(x => x.Name == query.Genre));

        if (!string.IsNullOrWhiteSpace(query.Actor))
            q = q.Where(m => m.Actors.Any(a => a.Name.Contains(query.Actor)));

        var total = await q.CountAsync();

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PagedResult<Movie>
        {
            Items = items,
            TotalItems = total
        };
    }

    public void Add(Movie movie)
        => context.Movies.Add(movie);

    public void Remove(Movie movie)
        => context.Movies.Remove(movie);
}