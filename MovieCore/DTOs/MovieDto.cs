/// <summary>Basic representation of a movie.</summary>
/// <example>
/// {
///   "id": 1,
///   "title": "Inception",
///   "year": 2010,
///   "shortDescription": "A thief enters dreams to steal secrets.",
///   "language": "English",
///   "duration": "148 min"
/// }
/// </example>
public class MovieDto
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

    /// <summary>Short description of the movie.</summary>
    /// <example>A thief enters dreams to steal secrets.</example>
    public string ShortDescription { get; set; } = string.Empty;

    /// <summary>Language of the movie.</summary>
    /// <example>English</example>
    public string Language { get; set; } = string.Empty;

    /// <summary>Duration of the movie.</summary>
    /// <example>148 min</example>
    public string Duration { get; set; } = string.Empty;
}