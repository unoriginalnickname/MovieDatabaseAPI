using Asp.Versioning;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class MoviesController(IServiceManager serviceManager, ILogger<MoviesController> _logger, LinkGenerator links) : ControllerBase
{
    /// <summary>Gets a movie by its ID.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <response code="200">Movie found.</response>
    /// <response code="404">Movie not found.</response>
    [HttpGet("{movieId}")]
    [Tags("Movies")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetMovieById(int movieId)
    {
        _logger.LogInformation("Getting movie {MovieId} (v1)", movieId);

        var movie = await serviceManager.MovieService.GetDetailsByIdAsync(movieId);

        if (movie == null)
            return Problem(detail: $"No movie found with id: {movieId}", statusCode: StatusCodes.Status404NotFound);

        //Just testing Level 3 REST in one endpoint:
        var reviewsUrl = links.GetPathByAction(
            HttpContext,
            action: "GetByMovieId",
            controller: "Review",
            values: new { movieId });

        return Ok(new
        {
            movie.Id,
            movie.Title,
            movie.Year,
            movie.ShortDescription,
            Links = new
            {
                Self = Url.Action(nameof(GetMovieById), new { movieId }),
                Reviews = reviewsUrl,
                Delete = Url.Action(nameof(DeleteMovie), new { movieId })
            }
        });
    }

    /// <summary>Gets detailed movie information.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <returns>The requested movie details.</returns>
    /// <response code="200">Movie found.</response>
    /// <response code="404">Movie not found.</response>
    [HttpGet("{movieId}")]
    [ApiExplorerSettings(GroupName = "Movies v2")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetMovieDetails(int movieId)
    {
        var movie = await serviceManager.MovieService.GetDetailsByIdAsync(movieId);

        return movie == null
            ? Problem(detail: $"No movie found with id: {movieId}", statusCode: StatusCodes.Status404NotFound)
            : Ok(movie);
    }
 
    /// <summary>Adds a review to a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="dto">Review data.</param>
    /// <response code="200">Review added.</response>
    /// <response code="400">Invalid request.</response>
    [HttpPost("{movieId}/reviews")]
    public async Task<IActionResult> AddReview(int movieId, [FromBody] CreateReviewDto dto)
    {
        var result = await serviceManager.MovieRelationService.AddReviewAsync(movieId, dto);

        return result.Success
            ? NoContent()
            : Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
    }

    /// <summary>Removes a review from a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="reviewId">Review ID.</param>
    /// <response code="204">Review removed.</response>
    /// <response code="404">Review not found.</response>
    [HttpDelete("{movieId}/reviews/{reviewId}")]
    public async Task<IActionResult> RemoveReview(int movieId, int reviewId)
    {
        var result = await serviceManager.MovieRelationService.RemoveReviewAsync(movieId, reviewId);

        return result.Success
            ? NoContent()
            : Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
    }
  
    /// <summary>Searches movies.</summary>
    /// <param name="query">Search criteria.</param>
    /// <returns>A paged list of movies.</returns>
    /// <response code="200">Movies returned.</response>
    [HttpGet]
    public async Task<IActionResult> GetMovies([FromQuery] MovieQuery query)
        => Ok(await serviceManager.MovieService.SearchAsync(query));

    /// <summary>Creates a new movie.</summary>
    /// <param name="dto">Movie data.</param>
    /// <returns>The created movie.</returns>
    /// <response code="201">Movie created successfully.</response>
    /// <response code="400">Invalid request.</response>
    [HttpPost]
    public async Task<IActionResult> AddMovie([FromBody] CreateMovieDto dto)
    {
        var result = await serviceManager.MovieService.CreateAsync(dto);

        if (!result.Success)
            return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);

        return CreatedAtAction(
            nameof(GetMovieById),
            new { movieId = result.Data!.Id },
            result.Data);
    }
    
    /// <summary>Updates a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="dto">Updated movie data.</param>
    /// <response code="204">Movie updated.</response>
    /// <response code="404">Movie not found.</response>
    [HttpPut("{movieId}")]
    public async Task<IActionResult> EditMovie(int movieId, [FromBody] UpdateMovieDto dto)
    {
        var result = await serviceManager.MovieService.UpdateAsync(movieId, dto);

        return result.Success
            ? NoContent()
            : Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
    }

    /// <summary>Partially updates a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="jsonPatch">Patch operations.</param>
    /// <returns>The updated movie.</returns>
    /// <response code="400">Invalid patch document or validation failure.</response>
    /// <response code="404">Movie not found.</response>
    /// <response code="200">Movie updated successfully.</response>
    [HttpPatch("{movieId}")]
    public async Task<IActionResult> PatchMovie(int movieId, [FromBody] JsonPatchDocument<PatchMovieDto> jsonPatch)
    {
        var patchMovieDto = await serviceManager.MovieService.GetPatchDto(movieId);

        if (patchMovieDto == null)
            return Problem(detail: $"No movie found with id: {movieId}", statusCode: StatusCodes.Status404NotFound);

        jsonPatch.ApplyTo(patchMovieDto);

        if (!ModelState.IsValid) //maybe good to explicitly do
            return BadRequest(ModelState);

        var result = await serviceManager.MovieService.PatchAsync(movieId, patchMovieDto);

        return result.Success
            ? Ok(result.Data)
            : Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
    }

    /// <summary>Deletes a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <response code="204">Movie deleted.</response>
    /// <response code="404">Movie not found.</response>
    [HttpDelete("{movieId}")]
    public async Task<IActionResult> DeleteMovie(int movieId)
    {
        var result = await serviceManager.MovieService.DeleteAsync(movieId);

        return result.Success
            ? NoContent()
            : Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
    }

    /// <summary>Adds an actor to a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="actorId">Actor ID.</param>
    /// <response code="200">Actor added.</response>
    /// <response code="400">Invalid request.</response>
    [HttpPost("{movieId}/actors/{actorId}")]
    public async Task<IActionResult> AddActor(int movieId, int actorId)
    {
        var result = await serviceManager.MovieRelationService.AddActorAsync(movieId, actorId);

        return result.Success
            ? NoContent()
            : Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
    }

    /// <summary>Removes an actor from a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="actorId">Actor ID.</param>
    /// <response code="204">Actor removed.</response>
    /// <response code="404">Actor or movie not found.</response>
    [HttpDelete("{movieId}/actors/{actorId}")]
    public async Task<IActionResult> RemoveActor(int movieId, int actorId)
    {
        var result = await serviceManager.MovieRelationService.RemoveActorAsync(movieId, actorId);

        return result.Success
            ? NoContent()
            : Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
    }

    /// <summary>Replaces all actors for a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="dto">List of actor IDs to assign.</param>
    /// <response code="204">Actors updated.</response>
    /// <response code="404">Movie not found.</response>
    [HttpPut("{movieId}/actors")]
    public async Task<IActionResult> SetActors(int movieId, [FromBody] SetActorsDto dto)
    {
        var result = await serviceManager.MovieRelationService.SetActorsAsync(movieId, dto.ActorIds);

        return result.Success
            ? NoContent()
            : Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
    }
 
    /// <summary>Replaces all genres for a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="genreIds">Genre IDs.</param>
    /// <response code="204">Genres updated.</response>
    /// <response code="404">Movie not found.</response>
    [HttpPut("{movieId}/genres")]
    public async Task<IActionResult> SetGenres(int movieId, [FromBody] List<int> genreIds)
    {
        var result = await serviceManager.MovieRelationService.SetGenresAsync(movieId, genreIds);

        return result.Success
            ? NoContent()
            : Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
    }
}
