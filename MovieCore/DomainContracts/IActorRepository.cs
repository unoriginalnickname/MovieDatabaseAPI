    public interface IActorRepository
    {
        public IQueryable<Actor> Query();
        Task<Actor?> GetAsync(int id);

        void Add(Actor actor);
        void Remove(Actor actor);
        Task<List<Actor>> GetByIdsAsync(List<int> ids);
    }
