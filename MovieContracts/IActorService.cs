public interface IActorService
{
    Task<ServiceResult<PagedResult<ActorDto>>> GetAllAsync(ActorQuery query);

    Task<ServiceResult<ActorDto>> GetByIdAsync(int id);

    Task<ServiceResult<ActorDto>> CreateAsync(CreateActorDto dto);

    Task<ServiceResult<ActorDto>> UpdateAsync(int id, UpdateActorDto dto);

    Task<ServiceResult> DeleteAsync(int id);
}