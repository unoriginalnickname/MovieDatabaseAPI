namespace MovieApi.Test
{
    public static class TestDataSeeder
    {
        public static void Seed(MovieDbContext ctx)
        {
            ctx.Actors.AddRange(
                new Actor { Name = "Leonardo DiCaprio" },
                new Actor { Name = "Brad Pitt" }
            );

            ctx.Genres.AddRange(
                new Genre { Name = "Action" },
                new Genre { Name = "Drama" }
            );

            ctx.SaveChanges();
        }
    }
}