public class MovieQuery : PagingQuery
{
    public string? Title { get; set; }

    public int? Year { get; set; }

    public string? Language { get; set; }

    public string? Genre { get; set; }

    public string? Actor { get; set; }

}