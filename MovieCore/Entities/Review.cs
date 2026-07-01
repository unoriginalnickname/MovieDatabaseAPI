using System.ComponentModel.DataAnnotations;

public class Review
{
    public int Id { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;

    [Range(1, 5, ErrorMessage = "Movie review score must be between 1 and 5")]
    public int? Rating { get; set; }
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
}
