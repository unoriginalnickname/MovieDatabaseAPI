public interface IGenreService
{
    Task<ServiceResult<PagedResult<GenreDto>>> GetAllAsync(GenreQuery query);

    Task<ServiceResult<GenreDto>> GetByIdAsync(int id);

    Task<ServiceResult<GenreDto>> CreateAsync(CreateGenreDto dto);

    Task<ServiceResult<GenreDto>> UpdateAsync(int id, UpdateGenreDto dto);

    Task<ServiceResult> DeleteAsync(int id);
}