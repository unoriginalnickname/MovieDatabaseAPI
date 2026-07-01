public interface IMovieService
{
    Task<ServiceResult<MovieDto>> GetByIdAsync(int id);

    Task<ServiceResult<MovieDetailsDto>> GetDetailsByIdAsync(int id);

    Task<ServiceResult<PagedResult<MovieDetailsDto>>> SearchAsync(MovieQuery query);

    Task<ServiceResult<MovieDetailsDto>> CreateAsync(CreateMovieDto dto);

    Task<ServiceResult<MovieDetailsDto>> UpdateAsync(int id, UpdateMovieDto dto);

    Task<ServiceResult<MovieDetailsDto>> PatchAsync(int id, PatchMovieDto dto);

    Task<ServiceResult> DeleteAsync(int id);

    Task<ServiceResult<PatchMovieDto>> GetPatchDto(int id);
}