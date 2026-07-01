using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

public class GenreService(IUnitOfWork uow, IMapper mapper) : IGenreService
{
    private readonly IConfigurationProvider _mapperConfig = mapper.ConfigurationProvider;

    public async Task<PagedResult<GenreDto>> GetAllAsync(GenreQuery query)
    {
        return await uow.Genres.Query()
            .ProjectTo<GenreDto>(_mapperConfig)
            .ToPagedResultAsync(query.Page, query.PageSize);
    }

    public async Task<GenreDto?> GetByIdAsync(int id)
    {
        return await uow.Genres.Query()
            .Where(x => x.Id == id)
            .ProjectTo<GenreDto>(_mapperConfig)
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<GenreDto>> CreateAsync(CreateGenreDto dto)
    {
        var genre = mapper.Map<Genre>(dto);

        await uow.Genres.AddAsync(genre);
        await uow.CompleteAsync();

        var result = mapper.Map<GenreDto>(genre);

        return ServiceResult<GenreDto>.Ok(result);
    }

    public async Task<ServiceResult<GenreDto>> UpdateAsync(int id, UpdateGenreDto dto)
    {
        var genre = await uow.Genres.GetAsync(id);

        if (genre == null)
            return ServiceResult<GenreDto>.Fail("Genre not found");

        mapper.Map(dto, genre);

        uow.Genres.Update(genre);
        await uow.CompleteAsync();

        var result = mapper.Map<GenreDto>(genre);

        return ServiceResult<GenreDto>.Ok(result);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var genre = await uow.Genres.GetAsync(id);

        if (genre == null)
            return ServiceResult.Fail("Genre not found");

        uow.Genres.Remove(genre);
        await uow.CompleteAsync();

        return ServiceResult.Ok();
    }
}