public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? Year { get; set; }
    public string ShortDescription { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public DateTime AddedAtDateTime { get; set; }
    public DateTime ChangedAtDateTime { get; set; }
    public string? FullDescription { get; set; } = string.Empty;
    public int? Budget { get; set; }

    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
