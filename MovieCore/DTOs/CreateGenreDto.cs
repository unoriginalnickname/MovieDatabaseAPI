/// <summary>
/// DTO used to create a new genre.
/// </summary>
public class CreateGenreDto
{
    /// <summary>
    /// Name of the genre.
    /// </summary>
    /// <example>Action</example>
    public string Name { get; set; } = string.Empty;
}