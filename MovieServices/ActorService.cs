using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class ActorService(IUnitOfWork uow, IMapper mapper, ILogger<ActorService> logger) : IActorService
{
    private readonly IConfigurationProvider _mapperConfig = mapper.ConfigurationProvider;

    public async Task<ServiceResult<PagedResult<ActorDto>>> GetAllAsync(ActorQuery query)
    {
        var result = await uow.Actors.Query()
            .ProjectTo<ActorDto>(_mapperConfig)
            .ToPagedResultAsync(query.Page, query.PageSize);

        return ServiceResultFactory.Ok(result);
    }

    public async Task<ServiceResult<ActorDto>> GetByIdAsync(int id)
    {
        var actor = await uow.Actors.Query()
            .Where(x => x.Id == id)
            .ProjectTo<ActorDto>(_mapperConfig)
            .FirstOrDefaultAsync();

        if (actor == null)
            return ServiceResultFactory.Fail<ActorDto>(
                logger,
                ErrorTypeEnum.NotFound,
                "Actor not found",
                nameof(GetByIdAsync));

        return ServiceResultFactory.Ok(actor);
    }

    public async Task<ServiceResult<ActorDto>> CreateAsync(CreateActorDto dto)
    {
        var actor = mapper.Map<Actor>(dto);

        uow.Actors.Add(actor);
        await uow.CompleteAsync();

        return ServiceResultFactory.Ok(mapper.Map<ActorDto>(actor));
    }

    public async Task<ServiceResult<ActorDto>> UpdateAsync(int id, UpdateActorDto dto)
    {
        var actor = await uow.Actors.GetAsync(id);

        if (actor == null)
            return ServiceResultFactory.Fail<ActorDto>(
                logger,
                ErrorTypeEnum.NotFound,
                "Actor not found",
                nameof(UpdateAsync));

        mapper.Map(dto, actor);

        await uow.CompleteAsync();

        return ServiceResultFactory.Ok(mapper.Map<ActorDto>(actor));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var actor = await uow.Actors.GetAsync(id);

        if (actor == null)
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.NotFound,
                "Actor not found",
                nameof(DeleteAsync));

        uow.Actors.Remove(actor);
        await uow.CompleteAsync();

        return ServiceResultFactory.Ok();
    }
}