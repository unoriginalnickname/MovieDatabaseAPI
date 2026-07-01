/// <summary>Represents a movie review.</summary>
/// <example>
/// {
///   "id": 1,
///   "author": "John Doe",
///   "comment": "Great movie with strong visuals.",
///   "rating": 5,
///   "movieId": 1,
///   "movieTitle": "Inception"
/// }
/// </example>
public class ReviewDto
{
    /// <summary>Unique identifier of the review.</summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>Name of the review author.</summary>
    /// <example>John Doe</example>
    public string Author { get; set; } = string.Empty;

    /// <summary>Review comment.</summary>
    /// <example>Great movie with strong visuals.</example>
    public string Comment { get; set; } = string.Empty;

    /// <summary>Rating given to the movie.</summary>
    /// <example>5</example>
    public int Rating { get; set; }

    /// <summary>Identifier of the related movie.</summary>
    /// <example>1</example>
    public int MovieId { get; set; }

    /// <summary>Title of the related movie.</summary>
    /// <example>Inception</example>
    public string MovieTitle { get; set; } = string.Empty;
}