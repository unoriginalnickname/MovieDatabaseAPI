    public class MovieDetailsWithLinksDto : MovieDetailsDto
    {
        public Dictionary<string, LinkDto> Links { get; set; } = [];
    }