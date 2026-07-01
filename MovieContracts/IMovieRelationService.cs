
    public interface IMovieRelationService
    {
        public Task<ServiceResult> AddReviewAsync(int movieId, CreateReviewDto dto);

        public Task<ServiceResult> RemoveReviewAsync(int movieId, int reviewId);

        Task<ServiceResult> AddActorAsync(int movieId, int actorId);
        Task<ServiceResult> RemoveActorAsync(int movieId, int actorId);

        Task<ServiceResult> SetActorsAsync(int movieId, List<int> actorIds);

        Task<ServiceResult> SetGenresAsync(int movieId, List<int> genreIds);
    }
