public interface IMovieService
{
    Task<MovieDto?> GetByIdAsync(int id);
    Task<MovieDetailsDto?> GetDetailsByIdAsync(int id);
    Task<PagedResult<MovieDetailsDto>> SearchAsync(MovieQuery query);
    Task<ServiceResult<MovieDetailsDto>> CreateAsync(CreateMovieDto dto);
    Task<ServiceResult<MovieDetailsDto>> UpdateAsync(int id, UpdateMovieDto dto);
    Task<ServiceResult<MovieDetailsDto>> PatchAsync(int id, PatchMovieDto dto);
    Task<PatchMovieDto?> GetPatchDto(int id);
    Task<ServiceResult> DeleteAsync(int id);
}
