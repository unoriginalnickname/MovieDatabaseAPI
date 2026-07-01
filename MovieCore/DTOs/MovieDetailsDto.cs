/// <summary>Detailed representation of a movie including actors, genres, and reviews.</summary>
/// <example>
/// {
///   "id": 1,
///   "title": "Inception",
///   "year": 2010,
///   "shortDescription": "A thief enters dreams to steal secrets.",
///   "fullDescription": "Full detailed description here...",
///   "language": "English",
///   "duration": "148 min",
///   "addedAt": "2024-01-01T12:00:00Z",
///   "budget": 160000000,
///   "actors": [],
///   "genres": [{ "id": 1, "name": "Action" }],
///   "reviews": []
/// }
/// </example>
public class MovieDetailsDto
{
    /// <summary>Unique identifier of the movie.</summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>Title of the movie.</summary>
    /// <example>Inception</example>
    public string Title { get; set; } = string.Empty;

    /// <summary>Release year of the movie.</summary>
    /// <example>2010</example>
    public int? Year { get; set; }

    /// <summary>Short summary of the movie.</summary>
    /// <example>A thief enters dreams to steal secrets.</example>
    public string ShortDescription { get; set; } = string.Empty;

    /// <summary>Full detailed description of the movie.</summary>
    /// <example>Full detailed description here...</example>
    public string FullDescription { get; set; } = string.Empty;

    /// <summary>Language of the movie.</summary>
    /// <example>English</example>
    public string Language { get; set; } = string.Empty;

    /// <summary>Duration of the movie.</summary>
    /// <example>148 min</example>
    public string Duration { get; set; } = string.Empty;

    /// <summary>Date and time the movie was added.</summary>
    /// <example>2024-01-01T12:00:00Z</example>
    public DateTime AddedAtDateTime { get; set; }
    
    /// <summary>Date and time the movie was updated or changed.</summary>
    /// <example>2024-01-01T12:00:00Z</example>
    public DateTime ChangedAtDateTime { get; set; }

    /// <summary>Production budget in USD.</summary>
    /// <example>160000000</example>
    public int? Budget { get; set; }

    /// <summary>List of actors in the movie.</summary>
    public List<ActorDto> Actors { get; set; } = [];

    /// <summary>List of genres associated with the movie.</summary>
    public List<GenreDto> Genres { get; set; } = [];

}