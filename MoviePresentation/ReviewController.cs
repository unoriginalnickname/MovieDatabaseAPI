using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Route("api")]
[ApiController]
public class ReviewController(IServiceManager serviceManager) : ControllerBase
{
    /// <summary>Retrieves all reviews.</summary>
    /// <param name="query">Pagination parameters.</param>
    /// <returns>A paged list of reviews.</returns>
    /// <response code="200">Reviews returned successfully.</response>
    [HttpGet("reviews")]
    public async Task<IActionResult> GetAll([FromQuery] PagingQuery query)
        => this.MapResult(await serviceManager.ReviewService.GetAllAsync(query));

    /// <summary>Gets reviews for a specific movie.</summary>
    /// <param name="movieId">Movie ID.</param>
    /// <param name="query">Pagination parameters.</param>
    /// <returns>A list of reviews for the movie.</returns>
    /// <response code="200">Reviews returned successfully.</response>
    /// <response code="404">Movie not found.</response>
    [HttpGet("movies/{movieId}/reviews")]
    public async Task<IActionResult> GetByMovieId(int movieId, [FromQuery] PagingQuery query)
        => this.MapResult(await serviceManager.ReviewService.GetByMovieIdAsync(movieId, query));


    /// <summary>Gets a review by ID.</summary>
    /// <param name="reviewId">Review ID.</param>
    /// <returns>The requested review.</returns>
    /// <response code="200">Review found.</response>
    /// <response code="404">Review not found.</response>
    [HttpGet("reviews/{reviewId}")]
    public async Task<IActionResult> GetById(int reviewId)
        => this.MapResult(await serviceManager.ReviewService.GetByIdAsync(reviewId));


    /// <summary>Deletes a review.</summary>
    /// <param name="reviewId">Review ID.</param>
    /// <response code="204">Review deleted successfully.</response>
    /// <response code="404">Review not found.</response>
    [HttpDelete("reviews/{reviewId}")]
    public async Task<IActionResult> Delete(int reviewId)
        => this.MapResult(await serviceManager.ReviewService.DeleteAsync(reviewId));
}
