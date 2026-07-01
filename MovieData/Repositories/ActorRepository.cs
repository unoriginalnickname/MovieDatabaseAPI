using Microsoft.EntityFrameworkCore;

public class ActorRepository(MovieDbContext context) : IActorRepository
{
    public IQueryable<Actor> Query() => context.Actors;

    public async Task<IEnumerable<Actor>> GetAllAsync()
        => await context.Actors.ToListAsync();

    public async Task<Actor?> GetAsync(int id)
        => await context.Actors.FindAsync(id);

    public async Task<bool> AnyAsync(int id)
        => await context.Actors.AnyAsync(a => a.Id == id);

    public void Add(Actor actor) => context.Actors.Add(actor);
    public void Update(Actor actor) => context.Actors.Update(actor);
    public void Remove(Actor actor) => context.Actors.Remove(actor);

    public async Task<List<Actor>> GetByIdsAsync(List<int> ids)
        => await context.Actors.Where(a => ids.Contains(a.Id)).ToListAsync();
}
