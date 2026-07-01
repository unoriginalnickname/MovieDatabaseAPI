using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class MovieService(
    IUnitOfWork uow,
    IMapper mapper,
    ILogger<MovieService> logger) : IMovieService
{
    public async Task<ServiceResult<PatchMovieDto>> GetPatchDto(int id)
    {
        var movie = await uow.Movies.Query()
            .Where(x => x.Id == id)
            .ProjectTo<PatchMovieDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
        if (movie == null)
            return ServiceResultFactory.Fail<PatchMovieDto>(
                logger,
                ErrorTypeEnum.NotFound,
                "Movie not found",
                nameof(GetPatchDto));

        return ServiceResultFactory.Ok(mapper.Map<PatchMovieDto>(movie));
    }

    public async Task<ServiceResult<PagedResult<MovieDetailsDto>>> SearchAsync(MovieQuery query)
    {
        var result = await uow.Movies.SearchAsync(query);

        var mapped = new PagedResult<MovieDetailsDto>
        {
            Items = mapper.Map<List<MovieDetailsDto>>(result.Items),
            TotalItems = result.TotalItems
        };

        return ServiceResultFactory.Ok(mapped);
    }

    public async Task<ServiceResult<MovieDto>> GetByIdAsync(int id)
    {
        var movie = await uow.Movies.GetByIdAsync(id);

        if (movie == null)
            return ServiceResultFactory.Fail<MovieDto>(
                logger,
                ErrorTypeEnum.NotFound,
                "Movie not found",
                nameof(GetByIdAsync));

        return ServiceResultFactory.Ok(mapper.Map<MovieDto>(movie));
    }

    public async Task<ServiceResult<MovieDetailsDto>> GetDetailsByIdAsync(int id)
    {
        var movie = await uow.Movies.GetWithDetailsAsync(id);

        if (movie == null)
            return ServiceResultFactory.Fail<MovieDetailsDto>(
                logger,
                ErrorTypeEnum.NotFound,
                "Movie not found",
                nameof(GetDetailsByIdAsync));

        return ServiceResultFactory.Ok(mapper.Map<MovieDetailsDto>(movie));
    }

    public async Task<ServiceResult<MovieDetailsDto>> CreateAsync(CreateMovieDto dto)
    {
        if (await uow.Movies.TitleExistsAsync(dto.Title))
            return ServiceResultFactory.Fail<MovieDetailsDto>(
                logger,
                ErrorTypeEnum.Conflict,
                $"A movie titled '{dto.Title}' already exists.",
                nameof(CreateAsync));

        var genres = await uow.Genres.GetManyAsync(dto.GenreIds);
        if (genres.Count != dto.GenreIds.Count)
            return ServiceResultFactory.Fail<MovieDetailsDto>(
                logger,
                ErrorTypeEnum.Validation,
                "One or more genres do not exist.",
                nameof(CreateAsync));

        var actors = await uow.Actors.GetByIdsAsync(dto.ActorIds);
        if (actors.Count != dto.ActorIds.Count)
            return ServiceResultFactory.Fail<MovieDetailsDto>(
                logger,
                ErrorTypeEnum.Validation,
                "One or more actors do not exist.",
                nameof(CreateAsync));

        var movie = mapper.Map<Movie>(dto);
        movie.AddedAtDateTime = DateTime.UtcNow;

        movie.Genres = genres;
        movie.Actors = actors;

        uow.Movies.Add(movie);
        await uow.CompleteAsync();

        var created = await uow.Movies.GetWithDetailsAsync(movie.Id);

        return ServiceResultFactory.Ok(mapper.Map<MovieDetailsDto>(created));
    }

    public async Task<ServiceResult<MovieDetailsDto>> UpdateAsync(int id, UpdateMovieDto dto)
    {
        var movie = await uow.Movies.GetWithDetailsAsync(id);

        if (movie == null)
            return ServiceResultFactory.Fail<MovieDetailsDto>(
                logger,
                ErrorTypeEnum.NotFound,
                "Movie not found",
                nameof(UpdateAsync));

        if (await uow.Movies.TitleExistsAsync(dto.Title, excludeId: id))
            return ServiceResultFactory.Fail<MovieDetailsDto>(
                logger,
                ErrorTypeEnum.Conflict,
                $"A movie titled '{dto.Title}' already exists.",
                nameof(UpdateAsync));

        var genres = await uow.Genres.GetManyAsync(dto.GenreIds);

        if (dto.Budget > 1_000_000 &&
            dto.ActorIds.Count > 10 &&
            genres.Any(g => g.Name == "Documentary"))
        {
            return ServiceResultFactory.Fail<MovieDetailsDto>(
                logger,
                ErrorTypeEnum.Validation,
                "A Documentary film cannot have more than 10 actors and a budget over 1 million.",
                nameof(UpdateAsync));
        }

        var actors = await uow.Actors.GetByIdsAsync(dto.ActorIds);

        movie.ChangedAtDateTime = DateTime.UtcNow;

        mapper.Map(dto, movie);

        movie.Genres = genres;
        movie.Actors = actors;

        await uow.CompleteAsync();

        return ServiceResultFactory.Ok(mapper.Map<MovieDetailsDto>(movie));
    }

    public async Task<ServiceResult<MovieDetailsDto>> PatchAsync(int id, PatchMovieDto dto)
    {
        var movie = await uow.Movies.GetWithDetailsAsync(id);

        if (movie == null)
            return ServiceResultFactory.Fail<MovieDetailsDto>(
                logger,
                ErrorTypeEnum.NotFound,
                "Movie not found",
                nameof(PatchAsync));

        if (dto.Title != null &&
            dto.Title != movie.Title &&
            await uow.Movies.TitleExistsAsync(dto.Title, excludeId: id))
        {
            return ServiceResultFactory.Fail<MovieDetailsDto>(
                logger,
                ErrorTypeEnum.Conflict,
                $"A movie titled '{dto.Title}' already exists.",
                nameof(PatchAsync));
        }

        mapper.Map(dto, movie);

        movie.ChangedAtDateTime = DateTime.UtcNow;

        await uow.CompleteAsync();

        return ServiceResultFactory.Ok(mapper.Map<MovieDetailsDto>(movie));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var movie = await uow.Movies.GetByIdAsync(id);

        if (movie == null)
            return ServiceResultFactory.Fail(
                logger,
                ErrorTypeEnum.NotFound,
                "Movie not found",
                nameof(DeleteAsync));

        uow.Movies.Remove(movie);
        await uow.CompleteAsync();

        return ServiceResultFactory.Ok();
    }
}