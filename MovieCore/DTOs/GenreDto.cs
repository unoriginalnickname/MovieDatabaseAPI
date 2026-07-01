/// <summary>Represents a genre.</summary>
public class GenreDto
{
    /// <summary>Unique identifier of the genre.</summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>Name of the genre.</summary>
    /// <example>Action</example>
    public string Name { get; set; } = string.Empty;
}