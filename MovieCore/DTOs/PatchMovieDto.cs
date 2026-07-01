using System.ComponentModel.DataAnnotations;

/// <summary>Request used to partially update a movie.</summary>
/// <example>
/// {
///   "title": "Inception",
///   "year": 2010,
///   "shortDescription": "A thief who enters dreams.",
///   "language": "English",
///   "duration": "148 min",
///   "genreIds": [1, 2],
///   "actorIds": [1, 2, 3]
/// }
/// </example>
public class PatchMovieDto
{
    /// <summary>Movie title.</summary>
    /// <example>Inception</example>
    [StringLength(50)]
    public string? Title { get; set; }

    /// <summary>Release year of the movie.</summary>
    /// <example>2010</example>
    [Range(1700, 2100)]
    public int? Year { get; set; }

    /// <summary>Short description of the movie.</summary>
    /// <example>A thief who enters dreams.</example>
    [StringLength(144)]
    public string? ShortDescription { get; set; }

    /// <summary>Primary language of the movie.</summary>
    /// <example>English</example>
    [StringLength(20)]
    public string? Language { get; set; }

    /// <summary>Duration of the movie.</summary>
    /// <example>148 min</example>
    public string? Duration { get; set; }

    /// <summary>List of genre IDs to associate with the movie.</summary>
    /// <example>[1, 2]</example>
    public List<int>? GenreIds { get; set; }

    /// <summary>List of actor IDs to associate with the movie.</summary>
    /// <example>[1, 2, 3]</example>
    public List<int>? ActorIds { get; set; }
}