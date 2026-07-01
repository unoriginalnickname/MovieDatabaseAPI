    public interface IActorRepository
    {
        public IQueryable<Actor> Query();
        Task<IEnumerable<Actor>> GetAllAsync();
        Task<Actor?> GetAsync(int id);
        Task<bool> AnyAsync(int id);

        void Add(Actor actor);
        void Update(Actor actor);
        void Remove(Actor actor);
        Task<List<Actor>> GetByIdsAsync(List<int> ids);
    }
