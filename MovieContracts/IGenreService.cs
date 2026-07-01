public interface IGenreService
{
    Task<PagedResult<GenreDto>> GetAllAsync(GenreQuery query);

    Task<GenreDto?> GetByIdAsync(int id);

    Task<ServiceResult<GenreDto>> CreateAsync(CreateGenreDto dto);

    Task<ServiceResult<GenreDto>> UpdateAsync(int id, UpdateGenreDto dto);

    Task<ServiceResult> DeleteAsync(int id);
}

