using Microsoft.EntityFrameworkCore;

public class MovieRepository(MovieDbContext context) : IMovieRepository
{

    public async Task<Movie?> GetMovieWithActorsAndGenresAsync(int movieId)
        => await context.Movies
            .Include(x => x.Actors)
            .Include(x => x.Genres)
            .FirstOrDefaultAsync(x => x.Id == movieId);

    public async Task<Movie?> GetAsync(int id)
        => await context.Movies
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Movie?> GetWithDetailsAsync(int id)
        => await context.Movies
            .Include(x => x.Actors)
            .Include(x => x.Genres)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Movie?> GetForPatchAsync(int id)
        => await context.Movies
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<bool> TitleExistsAsync(string title, int? excludeId = null)
        => await context.Movies.AnyAsync(m =>
            m.Title == title &&
            (excludeId == null || m.Id != excludeId));

    public async Task<PagedResult<Movie>> SearchAsync(MovieQuery query)
    {
        var movies = context.Movies
            .Include(x => x.Actors)
            .Include(x => x.Genres)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Title))
            movies = movies.Where(m => m.Title.Contains(query.Title));

        if (query.Year.HasValue)
            movies = movies.Where(m => m.Year == query.Year);

        if (!string.IsNullOrWhiteSpace(query.Language))
            movies = movies.Where(m => m.Language == query.Language);

        if (!string.IsNullOrWhiteSpace(query.Genre))
            movies = movies.Where(m => m.Genres.Any(x => x.Name == query.Genre));

        if (!string.IsNullOrWhiteSpace(query.Actor))
            movies = movies.Where(m => m.Actors.Any(a => a.Name.Contains(query.Actor)));

        var totalCount = await movies.CountAsync();

        var items = await movies
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PagedResult<Movie>
        {
            Items = items,
            TotalItems = totalCount
        };
    }

    public void Add(Movie movie)
        => context.Movies.Add(movie);

    public void Update(Movie movie)
        => context.Movies.Update(movie);

    public void Remove(Movie movie)
        => context.Movies.Remove(movie);
}