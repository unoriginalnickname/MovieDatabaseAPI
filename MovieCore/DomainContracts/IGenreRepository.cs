
    public interface IGenreRepository
    {
        public IQueryable<Genre> Query();
        Task<List<Genre>> GetAllAsync();
        Task<Genre?> GetAsync(int id);
        Task AddAsync(Genre genre);
        void Update(Genre genre);
        void Remove(Genre genre);
        Task<bool> AnyAsync(int id);
        Task<List<Genre>> GetManyAsync(List<int> ids);
    }
