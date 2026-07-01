using System.ComponentModel.DataAnnotations;

/// <summary>Request used to create a movie.</summary>
/// <example>
/// {
///   "title": "Inception",
///   "year": 2010,
///   "shortDescription": "A thief who steals corporate secrets through dream-sharing technology.",
///   "fullDescription": "Full movie description here...",
///   "language": "English",
///   "duration": "148 min",
///   "budget": 160000000,
///   "genreIds": [1, 2],
///   "actorIds": [1, 2]
/// }
/// </example>
public class CreateMovieDto
{
    /// <summary>Movie title.</summary>
    /// <example>Inception</example>
    [Required, StringLength(maximumLength: 50, MinimumLength = 1, ErrorMessage = "Title length must be within 0 and 50 characters")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Year the movie was released.</summary>
    /// <example>2010</example>
    [Range(1700, 2100)]
    public int? Year { get; set; }

    /// <summary>Short description of the movie.</summary>
    /// <example>A thief who steals corporate secrets through dream-sharing technology.</example>
    public string ShortDescription { get; set; } = string.Empty;

    /// <summary>Full detailed description of the movie.</summary>
    /// <example>Full movie description here...</example>
    public string? FullDescription { get; set; }

    /// <summary>Primary language of the movie.</summary>
    /// <example>English</example>
    public string Language { get; set; } = string.Empty;

    /// <summary>Duration of the movie.</summary>
    /// <example>148 min</example>
    public string Duration { get; set; } = string.Empty;

    /// <summary>Production budget in USD.</summary>
    /// <example>160000000</example>
    [Range(0, int.MaxValue, ErrorMessage = "Budget cannot be negative")]
    public int? Budget { get; set; }

    /// <summary>List of genre IDs associated with the movie.</summary>
    /// <example>[1, 2]</example>
    public List<int> GenreIds { get; set; } = [];

    /// <summary>List of Actor IDs associated with the movie.</summary>
    /// <example>[1, 2]</example>
    public List<int> ActorIds { get; set; } = [];
}