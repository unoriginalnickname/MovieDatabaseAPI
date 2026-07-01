using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/actors")]
public class ActorsController(IServiceManager serviceManager) : ControllerBase
{
    /// <summary>Retrieves a list of actors.</summary>
    /// <param name="query">Filtering and pagination parameters.</param>
    /// <returns>A list of actors.</returns>
    /// <response code="200">Actors returned successfully.</response>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ActorQuery query)
        => Ok(await serviceManager.ActorService.GetAllAsync(query));

    /// <summary>Gets an actor by ID.</summary>
    /// <param name="id">Actor ID.</param>
    /// <returns>The requested actor.</returns>
    /// <response code="200">Actor found.</response>
    /// <response code="404">Actor not found.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
        => this.MapResult(await serviceManager.ActorService.GetByIdAsync(id), $"No actor found with id: {id}");


    /// <summary>Creates a new actor.</summary>
    /// <param name="dto">Actor creation data.</param>
    /// <returns>The created actor.</returns>
    /// <response code="201">Actor created successfully.</response>
    /// <response code="400">Invalid request.</response>
    [HttpPost]
    public async Task<IActionResult> Create(CreateActorDto dto)
        => this.MapResult(await serviceManager.ActorService.CreateAsync(dto));


    /// <summary>Updates an existing actor.</summary>
    /// <param name="id">Actor ID.</param>
    /// <param name="dto">Updated actor data.</param>
    /// <response code="204">Actor updated successfully.</response>
    /// <response code="404">Actor not found.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateActorDto dto)
        => this.MapResult(await serviceManager.ActorService.UpdateAsync(id, dto));


    /// <summary>Deletes an actor.</summary>
    /// <param name="id">Actor ID.</param>
    /// <response code="204">Actor deleted successfully.</response>
    /// <response code="404">Actor not found.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
        => this.MapResult(await serviceManager.ActorService.DeleteAsync(id));
}
