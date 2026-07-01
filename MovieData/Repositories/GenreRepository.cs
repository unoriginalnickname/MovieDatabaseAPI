using Microsoft.EntityFrameworkCore;

public class GenreRepository : IGenreRepository
{
    private readonly MovieDbContext _context;

    public GenreRepository(MovieDbContext context)
    {
        _context = context;
    }
    public IQueryable<Genre> Query()
    {
        return _context.Genres;
    }

    public async Task<Genre?> GetAsync(int id)
        => await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);

    public async Task AddAsync(Genre genre)
        => await _context.Genres.AddAsync(genre);

    public void Remove(Genre genre)
        => _context.Genres.Remove(genre);

    public async Task<List<Genre>> GetManyAsync(List<int> ids)
    {
        return await _context.Genres
            .Where(g => ids.Contains(g.Id))
            .ToListAsync();
    }
}