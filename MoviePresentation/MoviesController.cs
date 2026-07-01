using Asp.Versioning;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

[Route("api/[controller]")] 
//[Route("api/v{version:apiVersion}/movies")]  // https://localhost:7259/api/v1/products
//[ApiVersion("1.0")]
//[ApiVersion("2.0")]
[ApiController]
public class MoviesController(IServiceManager serviceManager, ILogger<MoviesController> _logger, LinkGenerator links) : ControllerBase
{
    /// <summary>Adds an actor to a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="actorId">Actor ID.</param>
    /// <response code="200">Actor added.</response>
    /// <response code="400">Invalid request.</response>
    [HttpPost("{movieId}/actors/{actorId}")]
    public async Task<IActionResult> AddActor(int movieId, int actorId)
        => this.MapResult(await serviceManager.MovieRelationService.AddActorAsync(movieId, actorId));

    /// <summary>Creates a new movie.</summary>
    /// <param name="dto">Movie data.</param>
    /// <returns>The created movie.</returns>
    /// <response code="201">Movie created successfully.</response>
    /// <response code="400">Invalid request.</response>
    [HttpPost]
    public async Task<IActionResult> AddMovie(CreateMovieDto dto)
    {
        var result = await serviceManager.MovieService.CreateAsync(dto);

        return result.Success
            ? CreatedAtAction(nameof(GetMovieV1), new { movieId = result.Data!.Id }, result.Data)
            : this.MapResult(result);
    }

    /// <summary>Adds a review to a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="dto">Review data.</param>
    /// <response code="200">Review added.</response>
    /// <response code="400">Invalid request.</response>
    [HttpPost("{movieId}/reviews")]
    public async Task<IActionResult> AddReview(int movieId, CreateReviewDto dto)
        => this.MapResult(await serviceManager.MovieRelationService.AddReviewAsync(movieId, dto));



    /// <summary>Replaces all actors for a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="dto">List of actor IDs to assign.</param>
    /// <response code="204">Actors updated.</response>
    /// <response code="404">Movie not found.</response>
    [HttpPut("{movieId}/actors")]
    public async Task<IActionResult> SetActors(int movieId, SetActorsDto dto)
        => this.MapResult(await serviceManager.MovieRelationService.SetActorsAsync(movieId, dto.ActorIds));


    /// <summary>Replaces all genres for a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="genreIds">Genre IDs.</param>
    /// <response code="204">Genres updated.</response>
    /// <response code="404">Movie not found.</response>
    [HttpPut("{movieId}/genres")]
    public async Task<IActionResult> SetGenres(int movieId, List<int> genreIds)
        => this.MapResult(await serviceManager.MovieRelationService.SetGenresAsync(movieId, genreIds));

    /// <summary>Updates a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="dto">Updated movie data.</param>
    /// <response code="204">Movie updated.</response>
    /// <response code="404">Movie not found.</response>
    [HttpPut("{movieId}")]
    public async Task<IActionResult> EditMovie(int movieId, UpdateMovieDto dto)
        => this.MapResult(await serviceManager.MovieService.UpdateAsync(movieId, dto));

    /// <summary>Partially updates a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="jsonPatch">Patch operations.</param>
    /// <returns>The updated movie.</returns>
    /// <response code="400">Invalid patch document or validation failure.</response>
    /// <response code="404">Movie not found.</response>
    /// <response code="200">Movie updated successfully.</response>
    [HttpPatch("{movieId}")]
    public async Task<IActionResult> PatchMovie(
        int movieId,
        JsonPatchDocument<PatchMovieDto> patch)
    {
        var dtoResult = await serviceManager.MovieService.GetPatchDto(movieId);

        if (!dtoResult.Success)
            return this.MapResult(dtoResult);

        patch.ApplyTo(dtoResult.Data!);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await serviceManager.MovieService.PatchAsync(movieId, dtoResult.Data!);
        return this.MapResult(result);
    }




    /// <summary>Gets a movie by its ID.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <response code="200">Movie found.</response>
    /// <response code="404">Movie not found.</response>

    [HttpGet("{movieId}")]
    public async Task<IActionResult> GetMovieV1(int movieId)
    {
        _logger.LogInformation("Getting movie {MovieId} (v1)", movieId);

        var result = await serviceManager.MovieService.GetDetailsByIdAsync(movieId);

        if (!result.Success)
            return this.MapResult(result);

        var movie = result.Data!;

        //Just testing Level 3 REST
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
                Self = Url.Action(nameof(GetMovieV1), new { movieId }),
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
    [HttpGet("{movieId}/details")]
    //[MapToApiVersion("2.0")]
    public async Task<IActionResult> GetMovieV2(int movieId)
        => this.MapResult(await serviceManager.MovieService.GetDetailsByIdAsync(movieId));

    /// <summary>Search function. Searches for specified movie details</summary>
    /// <param name="query">Search criteria.</param>
    /// <returns>A paged list of movies.</returns>
    /// <response code="200">Movies returned.</response>
    [HttpGet]
    public async Task<IActionResult> GetMovies([FromQuery] MovieQuery query)
        => this.MapResult(await serviceManager.MovieService.SearchAsync(query));

    /// <summary>Removes a review from a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="reviewId">Review ID.</param>
    /// <response code="204">Review removed.</response>
    /// <response code="404">Review not found.</response>
    [HttpDelete("{movieId}/reviews/{reviewId}")]
    public async Task<IActionResult> RemoveReview(int movieId, int reviewId)
        => this.MapResult(await serviceManager.MovieRelationService.RemoveReviewAsync(movieId, reviewId));

    /// <summary>Deletes a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <response code="204">Movie deleted.</response>
    /// <response code="404">Movie not found.</response>
    [HttpDelete("{movieId}")]
    public async Task<IActionResult> DeleteMovie(int movieId)
        => this.MapResult(await serviceManager.MovieService.DeleteAsync(movieId));

    /// <summary>Removes an actor from a movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="actorId">Actor ID.</param>
    /// <response code="204">Actor removed.</response>
    /// <response code="404">Actor or movie not found.</response>
    [HttpDelete("{movieId}/actors/{actorId}")]
    public async Task<IActionResult> RemoveActor(int movieId, int actorId)
        => this.MapResult(await serviceManager.MovieRelationService.RemoveActorAsync(movieId, actorId));

}
