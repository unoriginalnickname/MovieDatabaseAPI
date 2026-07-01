/// <summary>Request used to update a genre.</summary>
/// <example>
/// {
///   "name": "Action"
/// }
/// </example>
public class UpdateGenreDto
{
    /// <summary>Name of the genre.</summary>
    /// <example>Action</example>
    public string Name { get; set; } = string.Empty;
}