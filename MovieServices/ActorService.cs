using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

public class ActorService(IUnitOfWork uow, IMapper mapper) : IActorService
{
    private readonly IConfigurationProvider _mapperConfig = mapper.ConfigurationProvider;

    public async Task<PagedResult<ActorDto>> GetAllAsync(ActorQuery query)
        => await uow.Actors.Query()
            .ProjectTo<ActorDto>(_mapperConfig)
            .ToPagedResultAsync(query.Page, query.PageSize);

    public async Task<ActorDto?> GetByIdAsync(int id)
        => await uow.Actors.Query()
            .Where(x => x.Id == id)
            .ProjectTo<ActorDto>(_mapperConfig)
            .FirstOrDefaultAsync();

    public async Task<ServiceResult<ActorDto>> CreateAsync(CreateActorDto dto)
    {
        var actor = mapper.Map<Actor>(dto);

        uow.Actors.Add(actor);
        await uow.CompleteAsync();

        return ServiceResult<ActorDto>.Ok(mapper.Map<ActorDto>(actor));
    }

    public async Task<ServiceResult<ActorDto>> UpdateAsync(int id, UpdateActorDto dto)
    {
        var actor = await uow.Actors.GetAsync(id);

        if (actor == null)
            return ServiceResult<ActorDto>.Fail("Actor not found");

        mapper.Map(dto, actor);

        uow.Actors.Update(actor);
        await uow.CompleteAsync();

        return ServiceResult<ActorDto>.Ok(mapper.Map<ActorDto>(actor));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var actor = await uow.Actors.GetAsync(id);

        if (actor == null)
            return ServiceResult.Fail("Actor not found");

        uow.Actors.Remove(actor);
        await uow.CompleteAsync();

        return ServiceResult.Ok();
    }
}
