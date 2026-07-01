    public interface IActorService
    {
        public Task<PagedResult<ActorDto>> GetAllAsync(ActorQuery query);
        Task<ActorDto?> GetByIdAsync(int id);
        Task<ServiceResult<ActorDto>> CreateAsync(CreateActorDto dto);
        Task<ServiceResult<ActorDto>> UpdateAsync(int id, UpdateActorDto dto);
        public Task<ServiceResult> DeleteAsync(int id);

    }
