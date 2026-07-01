/// <summary>Request used to create a movie review.</summary>
/// <example>
/// {
///   "author": "John Doe",
///   "comment": "Great movie with strong visuals.",
///   "rating": 5
/// }
/// </example>
public class CreateReviewDto
{
    /// <summary>Name of the review author.</summary>
    /// <example>John Doe</example>
    public string Author { get; set; } = string.Empty;

    /// <summary>Review comment.</summary>
    /// <example>Great movie with strong visuals.</example>
    public string Comment { get; set; } = string.Empty;

    /// <summary>Rating given to the movie.</summary>
    /// <example>5</example>
    public int Rating { get; set; }
}