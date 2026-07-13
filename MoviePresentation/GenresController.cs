using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class GenresController(IServiceManager serviceManager) : ControllerBase
{
    /// <summary>Retrieves a list of genres.</summary>
    /// <param name="query">Filtering and pagination parameters.</param>
    /// <returns>A list of genres.</returns>
    /// <response code="200">Genres returned successfully.</response>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GenreQuery query)
        => this.MapResult(await serviceManager.GenreService.GetAllAsync(query));

    /// <summary>Gets a genre by ID.</summary>
    /// <param name="genreId">Genre ID.</param>
    /// <returns>The requested genre.</returns>
    /// <response code="200">Genre found.</response>
    /// <response code="404">Genre not found.</response>
    [HttpGet("{genreId}")]
    public async Task<IActionResult> GetById(int genreId)
        => this.MapResult(await serviceManager.GenreService.GetByIdAsync(genreId));

    /// <summary>Creates a new genre.</summary>
    /// <param name="dto">Genre creation data.</param>
    /// <returns>The created genre.</returns>
    /// <response code="200">Genre created successfully.</response>
    [HttpPost]
    public async Task<IActionResult> Create(CreateGenreDto dto)
        => this.MapResult(await serviceManager.GenreService.CreateAsync(dto));

    /// <summary>Updates an existing genre.</summary>
    /// <param name="genreId">Genre ID.</param>
    /// <param name="dto">Updated genre data.</param>
    /// <returns>The updated genre.</returns>
    /// <response code="200">Genre updated successfully.</response>
    /// <response code="404">Genre not found.</response>
    [HttpPut("{genreId}")]
    public async Task<IActionResult> Update(int genreId, UpdateGenreDto dto)
        => this.MapResult(await serviceManager.GenreService.UpdateAsync(genreId, dto));

    /// <summary>Deletes a genre.</summary>
    /// <param name="genreId">Genre ID.</param>
    /// <response code="204">Genre deleted successfully.</response>
    /// <response code="404">Genre not found.</response>
    [HttpDelete("{genreId}")]
    public async Task<IActionResult> Delete(int genreId)
        => this.MapResult(await serviceManager.GenreService.DeleteAsync(genreId));
}
