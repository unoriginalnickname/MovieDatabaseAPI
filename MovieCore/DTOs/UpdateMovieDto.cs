using System.ComponentModel.DataAnnotations;

/// <summary>Request used to fully update a movie (replacement update).</summary>
/// <example>
/// {
///   "title": "Inception",
///   "year": 2010,
///   "shortDescription": "A thief enters dreams.",
///   "fullDescription": "Full movie description here...",
///   "language": "English",
///   "budget": 160000000,
///   "duration": "148 min",
///   "genreIds": [1, 2],
///   "actorIds": [1, 2, 3]
/// }
/// </example>
public class UpdateMovieDto
{
    /// <summary>Movie title.</summary>
    /// <example>Inception</example>
    public string Title { get; set; } = null!;

    /// <summary>Release year of the movie.</summary>
    /// <example>2010</example>
    public int? Year { get; set; }

    /// <summary>Short description of the movie.</summary>
    /// <example>A thief enters dreams.</example>
    public string ShortDescription { get; set; } = null!;

    /// <summary>Full detailed description of the movie.</summary>
    /// <example>Full movie description here...</example>
    public string FullDescription { get; set; } = null!;

    /// <summary>Primary language of the movie.</summary>
    /// <example>English</example>
    public string Language { get; set; } = null!;

    /// <summary>Production budget in USD.</summary>
    /// <example>160000000</example>
    [Range(0, int.MaxValue, ErrorMessage = "Budget cannot be negative")]
    public int? Budget { get; set; }

    /// <summary>Duration of the movie.</summary>
    /// <example>148 min</example>
    public string Duration { get; set; } = null!;

    /// <summary>List of genre IDs associated with the movie.</summary>
    /// <example>[1, 2]</example>
    public List<int> GenreIds { get; set; } = new();

    /// <summary>List of actor IDs associated with the movie.</summary>
    /// <example>[1, 2, 3]</example>
    public List<int> ActorIds { get; set; } = new();
}