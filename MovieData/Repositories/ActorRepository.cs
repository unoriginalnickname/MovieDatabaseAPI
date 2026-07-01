using Microsoft.EntityFrameworkCore;

public class ActorRepository(MovieDbContext context) : IActorRepository
{
    public IQueryable<Actor> Query() => context.Actors;

    public async Task<Actor?> GetAsync(int id)
        => await context.Actors.FindAsync(id);

    public void Add(Actor actor) => context.Actors.Add(actor);
    public void Remove(Actor actor) => context.Actors.Remove(actor);

    public async Task<List<Actor>> GetByIdsAsync(List<int> ids)
        => await context.Actors.Where(a => ids.Contains(a.Id)).ToListAsync();
}
