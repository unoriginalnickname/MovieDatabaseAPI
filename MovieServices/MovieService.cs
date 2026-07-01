using AutoMapper;

public class MovieService(IUnitOfWork uow, IMapper mapper) : IMovieService
{
    public async Task<PatchMovieDto?> GetPatchDto(int id)
    {
        var movie = await uow.Movies.GetForPatchAsync(id);
        return movie == null ? null : mapper.Map<PatchMovieDto>(movie);
    }

    public async Task<PagedResult<MovieDetailsDto>> SearchAsync(MovieQuery query)
    {
        var result = await uow.Movies.SearchAsync(query);

        return new PagedResult<MovieDetailsDto>
        {
            Items = mapper.Map<List<MovieDetailsDto>>(result.Items),
            TotalItems = result.TotalItems
        };
    }

    public async Task<MovieDto?> GetByIdAsync(int id)
    {
        var movie = await uow.Movies.GetAsync(id);
        return movie == null ? null : mapper.Map<MovieDto>(movie);
    }

    public async Task<MovieDetailsDto?> GetDetailsByIdAsync(int id)
    {
        var movie = await uow.Movies.GetWithDetailsAsync(id);
        return movie == null ? null : mapper.Map<MovieDetailsDto>(movie);
    }

    public async Task<ServiceResult<MovieDetailsDto>> CreateAsync(CreateMovieDto dto)
    {
        if (await uow.Movies.TitleExistsAsync(dto.Title))
            return ServiceResult<MovieDetailsDto>.Fail($"A movie titled '{dto.Title}' already exists.");

        var movie = mapper.Map<Movie>(dto);
        movie.AddedAtDateTime = DateTime.UtcNow;

        var genres = await uow.Genres.GetManyAsync(dto.GenreIds);
        if (genres.Count != dto.GenreIds.Count)
            return ServiceResult<MovieDetailsDto>.Fail("One or more genres do not exist.");

        var actors = await uow.Actors.GetByIdsAsync(dto.ActorIds);
        if (actors.Count != dto.ActorIds.Count)
            return ServiceResult<MovieDetailsDto>.Fail("One or more actors do not exist.");

        movie.Genres = genres;
        movie.Actors = actors;

        uow.Movies.Add(movie);
        await uow.CompleteAsync();

        var created = await uow.Movies.GetWithDetailsAsync(movie.Id);

        return ServiceResult<MovieDetailsDto>.Ok(mapper.Map<MovieDetailsDto>(created));
    }

    public async Task<ServiceResult<MovieDetailsDto>> UpdateAsync(int id, UpdateMovieDto dto)
    {
        var movie = await uow.Movies.GetWithDetailsAsync(id);

        if (movie == null)
            return ServiceResult<MovieDetailsDto>.Fail("Movie not found");

        if (await uow.Movies.TitleExistsAsync(dto.Title, excludeId: id))
            return ServiceResult<MovieDetailsDto>.Fail($"A movie titled '{dto.Title}' already exists.");

        var genres = await uow.Genres.GetManyAsync(dto.GenreIds);

        if (dto.Budget > 1_000_000 &&
            dto.ActorIds.Count > 10 &&
            genres.Any(g => g.Name == "Documentary"))
        {
            return ServiceResult<MovieDetailsDto>.Fail(
                "A Documentary film cannot have more than 10 actors and a budget over 1 million.");
        }

        var actors = await uow.Actors.GetByIdsAsync(dto.ActorIds);

        movie.ChangedAtDateTime = DateTime.UtcNow;

        mapper.Map(dto, movie);

        movie.Genres = genres;
        movie.Actors = actors;

        uow.Movies.Update(movie);
        await uow.CompleteAsync();

        return ServiceResult<MovieDetailsDto>.Ok(mapper.Map<MovieDetailsDto>(movie));
    }

    public async Task<ServiceResult<MovieDetailsDto>> PatchAsync(int id, PatchMovieDto dto)
    {
        var movie = await uow.Movies.GetWithDetailsAsync(id);

        if (movie == null)
            return ServiceResult<MovieDetailsDto>.Fail("Movie not found");

        if (dto.Title != null &&
            dto.Title != movie.Title &&
            await uow.Movies.TitleExistsAsync(dto.Title, excludeId: id))
        {
            return ServiceResult<MovieDetailsDto>.Fail($"A movie titled '{dto.Title}' already exists.");
        }

        mapper.Map(dto, movie);

        movie.ChangedAtDateTime = DateTime.UtcNow;

        uow.Movies.Update(movie);
        await uow.CompleteAsync();

        return ServiceResult<MovieDetailsDto>.Ok(mapper.Map<MovieDetailsDto>(movie));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var movie = await uow.Movies.GetAsync(id);

        if (movie == null)
            return ServiceResult.Fail("Movie not found");

        uow.Movies.Remove(movie);
        await uow.CompleteAsync();

        return ServiceResult.Ok();
    }
}