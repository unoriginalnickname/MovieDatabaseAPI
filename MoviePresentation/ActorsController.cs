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
    {
        var actor = await serviceManager.ActorService.GetByIdAsync(id);

        return actor == null
            ? Problem(detail: $"No actor found with id: {id}", statusCode: StatusCodes.Status404NotFound)
            : Ok(actor);
    }

    /// <summary>Creates a new actor.</summary>
    /// <param name="dto">Actor creation data.</param>
    /// <returns>The created actor.</returns>
    /// <response code="201">Actor created successfully.</response>
    /// <response code="400">Invalid request.</response>
    [HttpPost]
    public async Task<IActionResult> Create(CreateActorDto dto)
    {
        var result = await serviceManager.ActorService.CreateAsync(dto);

        if (!result.Success)
            return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
            result.Data);
    }

    /// <summary>Updates an existing actor.</summary>
    /// <param name="id">Actor ID.</param>
    /// <param name="dto">Updated actor data.</param>
    /// <response code="204">Actor updated successfully.</response>
    /// <response code="404">Actor not found.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateActorDto dto)
    {
        var result = await serviceManager.ActorService.UpdateAsync(id, dto);

        return result.Success
            ? NoContent()
            : Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
    }

    /// <summary>Deletes an actor.</summary>
    /// <param name="id">Actor ID.</param>
    /// <response code="204">Actor deleted successfully.</response>
    /// <response code="404">Actor not found.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await serviceManager.ActorService.DeleteAsync(id);

        return result.Success
            ? NoContent()
            : Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
    }
}
