using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

    public static class SeedExtension
    {
        public static async Task SeedData(this WebApplication application)
        {
            using var scope = application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MovieDbContext>();

            if (context.Movies.Any()) return;

            var actors = new List<Actor>
                {
                    new Actor { Name = "Jubba Jones", BirthYear = 1337, Description = "Jubba Jones has some description" },
                    new Actor { Name = "Flimma Blimma", BirthYear = 1338, Description = "Flimma Blimma has some description" },

                    new Actor { Name = "Elias Storm", BirthYear = 1984, Description = "Known for intense dramatic roles in indie thrillers" },
                    new Actor { Name = "Mira Solberg", BirthYear = 1991, Description = "Swedish actress with a background in theatre" },
                    new Actor { Name = "Noah Vinter", BirthYear = 1979, Description = "Action film veteran with stunt training" },
                    new Actor { Name = "Luna Hart", BirthYear = 2000, Description = "Rising star in sci-fi television series" },
                    new Actor { Name = "Oskar Nyström", BirthYear = 1988, Description = "Comedy actor known for improvisation skills" },
                    new Actor { Name = "Ava Linden", BirthYear = 1995, Description = "Appears in psychological dramas and mystery films" },
                    new Actor { Name = "Victor Hale", BirthYear = 1972, Description = "Classic supporting actor in crime movies" },
                    new Actor { Name = "Sofia Berg", BirthYear = 1993, Description = "Known for romantic comedies and indie films" },
                    new Actor { Name = "Daniel Kross", BirthYear = 1981, Description = "Sci-fi and fantasy genre specialist" },
                    new Actor { Name = "Elin Frost", BirthYear = 1997, Description = "Breakthrough role in dystopian TV series" }
                };
                var genres = new List<Genre>
                {
                    new Genre { Name = "Documentary" },
                    new Genre { Name = "Sci-Fi" },
                    new Genre { Name = "Comedy" },

                    new Genre { Name = "Drama" },
                    new Genre { Name = "Action" },
                    new Genre { Name = "Thriller" },
                    new Genre { Name = "Horror" },
                    new Genre { Name = "Romance" },
                    new Genre { Name = "Fantasy" },
                    new Genre { Name = "Mystery" },
                    new Genre { Name = "Crime" },
                    new Genre { Name = "Animation" },
                    new Genre { Name = "Adventure" }
                };
                var reviewTemplates = new[]
                {
                    ("Great dystopian sci-fi comedy mix, surprisingly good worldbuilding", 5, "Critic One"),
                    ("Funny but also strangely dark at times", 4, "Critic Two"),
                    ("Strong performances but weak pacing", 3, "FilmWatcher"),
                    ("Visually stunning and emotionally engaging", 5, "CinemaDaily"),
                    ("Script felt a bit predictable", 3, "MovieLensUser"),
                    ("Unexpectedly deep character development", 5, "IndieCritic"),
                    ("Good concept but execution could be better", 3, "ReviewGuy"),
                    ("One of the best films this year", 5, "TopCritic"),
                    ("Decent but forgettable", 2, "AverageViewer"),
                    ("Excellent soundtrack and atmosphere", 4, "SoundFilmFan"),
                    ("Too long, but still enjoyable", 4, "CasualWatcher")
                };
                var random = new Random();
                List<Review> CreateReviews(int count)
                {
                    return reviewTemplates
                        .OrderBy(_ => random.Next())
                        .Take(count)
                        .Select(r => new Review
                        {
                            Comment = r.Item1,
                            Rating = r.Item2,
                            Author = r.Item3,
                        })
                        .ToList();
                }

                List<T> PickRandom<T>(List<T> source, int min, int max)
                {
                    return source
                        .OrderBy(_ => random.Next())
                        .Take(random.Next(min, max + 1))
                        .ToList();
                }

                var movieDefinitions = new[]
                {
    new { Title = "Neon Collapse", Year = 2024, Budget = 2000000, Duration = "2h 10m", Language = "English", Short = "Cyberpunk collapse story", Full = "Cyberpunk city falls into chaos" },
    new { Title = "Silent Orbit", Year = 2023, Budget = 1500000, Duration = "1h 50m", Language = "English", Short = "Lost in space thriller", Full = "Space mission gone silent" },
    new { Title = "Frostline", Year = 2022, Budget = 3000000, Duration = "2h 5m", Language = "English", Short = "Arctic survival drama", Full = "Survival in frozen wasteland" },
    new { Title = "Echo Protocol", Year = 2025, Budget = 5000000, Duration = "2h 20m", Language = "English", Short = "AI thriller", Full = "AI system becomes self-aware" },
    new { Title = "Broken Signal", Year = 2021, Budget = 1200000, Duration = "1h 40m", Language = "English", Short = "Conspiracy mystery", Full = "Lost transmission reveals conspiracy" },
    new { Title = "Last Colony", Year = 2026, Budget = 4000000, Duration = "2h 30m", Language = "English", Short = "Space colony survival", Full = "Humanity's final space settlement" },
    new { Title = "Iron Dream", Year = 2020, Budget = 2500000, Duration = "2h", Language = "English", Short = "AI war story", Full = "War between machines and humans" },
    new { Title = "Glass Horizon", Year = 2024, Budget = 1800000, Duration = "1h 55m", Language = "English", Short = "Dystopian city story", Full = "A city behind a transparent dome" },
    new { Title = "Darkwater", Year = 2023, Budget = 2200000, Duration = "2h 15m", Language = "English", Short = "Underwater mystery", Full = "Mystery beneath ocean colony" },
    new { Title = "Quantum Fade", Year = 2025, Budget = 3500000, Duration = "2h 25m", Language = "English", Short = "Quantum sci-fi thriller", Full = "Reality distortion experiments" },

    new { Title = "Signal Drift", Year = 2022, Budget = 900000, Duration = "1h 35m", Language = "English", Short = "Lost satellite signals", Full = "Global communications failure" },
    new { Title = "Ashen Skies", Year = 2021, Budget = 1100000, Duration = "1h 45m", Language = "English", Short = "Post-apocalyptic flight story", Full = "Pilots surviving fallout skies" },
    new { Title = "Vector Prime", Year = 2024, Budget = 2600000, Duration = "2h 5m", Language = "English", Short = "Math-driven sci-fi", Full = "Reality controlled by algorithms" },
    new { Title = "Cold Memory", Year = 2020, Budget = 800000, Duration = "1h 30m", Language = "English", Short = "Memory loss mystery", Full = "A man reconstructs his past" },
    new { Title = "Nova Theory", Year = 2025, Budget = 6000000, Duration = "2h 40m", Language = "English", Short = "Cosmic discovery", Full = "First contact with deep space intelligence" }
};
                for (int i = 0; i < 3; i++)
                {
                    movieDefinitions = movieDefinitions.Concat(movieDefinitions).ToArray();
                }
                var movies = movieDefinitions.Select(m => new Movie
                {
                    Title = m.Title,
                    AddedAtDateTime = DateTime.UtcNow,
                    Duration = m.Duration,
                    ShortDescription = m.Short,
                    Language = m.Language,
                    Year = m.Year,
                    Genres = PickRandom(genres, 1, 3),
                    FullDescription = m.Full,
                    Budget = m.Budget,
                }).ToList();

                context.Movies.AddRange(movies);
                await context.SaveChangesAsync();

                foreach (var movie in context.Movies)
                {
                    movie.Reviews = CreateReviews(random.Next(5, 12));
                    movie.Actors = PickRandom(actors, 3, 5);
                }
                await context.SaveChangesAsync();
        }
    }
