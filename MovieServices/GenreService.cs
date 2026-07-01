using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class GenreService(IUnitOfWork uow, IMapper mapper, ILogger<GenreService> logger) : IGenreService
{
    private readonly IConfigurationProvider _mapperConfig = mapper.ConfigurationProvider;

    public async Task<ServiceResult<PagedResult<GenreDto>>> GetAllAsync(GenreQuery query)
    {
        var result = await uow.Genres.Query()
            .ProjectTo<GenreDto>(_mapperConfig)
            .ToPagedResultAsync(query.Page, query.PageSize);

        return ServiceResultFactory.Ok(result);
    }

    public async Task<ServiceResult<GenreDto>> GetByIdAsync(int id)
    {
        var genre = await uow.Genres.Query()
            .Where(x => x.Id == id)
            .ProjectTo<GenreDto>(_mapperConfig)
            .FirstOrDefaultAsync();

        if (genre == null)
            return ServiceResultFactory.Fail<GenreDto>(
                logger,
                ErrorTypeEnum.NotFound,
                "Genre not found",
                nameof(GetByIdAsync));

        return ServiceResultFactory.Ok(genre);
    }

    public async Task<ServiceResult<GenreDto>> CreateAsync(CreateGenreDto dto)
    {
        var genre = mapper.Map<Genre>(dto);

        uow.Genres.AddAsync(genre);
        await uow.CompleteAsync();

        return ServiceResultFactory.Ok(mapper.Map<GenreDto>(genre));
    }

    public async Task<ServiceResult<GenreDto>> UpdateAsync(int id, UpdateGenreDto dto)
    {
        var genre = await uow.Genres.GetAsync(id);

        if (genre == null)
            return ServiceResultFactory.Fail<GenreDto>(
                logger,
                ErrorTypeEnum.NotFound,
                "Genre not found",
                nameof(UpdateAsync));

        mapper.Map(dto, genre);

        await uow.CompleteAsync();

        return ServiceResultFactory.Ok(mapper.Map<GenreDto>(genre));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var genre = await uow.Genres.GetAsync(id);

        if (genre == null)
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.NotFound,
                "Genre not found",
                nameof(DeleteAsync));

        uow.Genres.Remove(genre);
        await uow.CompleteAsync();

        return ServiceResultFactory.Ok();
    }
}