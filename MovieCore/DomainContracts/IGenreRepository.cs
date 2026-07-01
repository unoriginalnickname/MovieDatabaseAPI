
    public interface IGenreRepository
    {
        public IQueryable<Genre> Query();
        Task<Genre?> GetAsync(int id);
        Task AddAsync(Genre genre);
        void Remove(Genre genre);
        Task<List<Genre>> GetManyAsync(List<int> ids);
    }
