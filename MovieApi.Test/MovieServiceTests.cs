using AutoMapper;
using EFCorePracticeProject.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MovieApi.Test;


/// <summary>
/// Unit tests for MovieService covering create, delete, and update behavior using an in-memory database.
/// </summary>
public class MovieServiceTests
{

    [Fact]
    public async Task CreateAsync_CanAddMovie()
    {
        using var ctx = CreateContext();

        var _movieService = MakeHelper.CreateMovieServiceWithRealMapper(ctx);

        TestDataSeeder.Seed(ctx);

        var initialAmountOfMovies = ctx.Movies.Count();

        var movieDto = MakeHelper.MakeCreateMovieDto();

        await _movieService.CreateAsync(movieDto);
        await ctx.SaveChangesAsync();

        Assert.True(ctx.Movies.Count() == initialAmountOfMovies + 1);
        var result = await _movieService.GetDetailsByIdAsync(1);

        var addedMovie = result.Data;

        Assert.True(addedMovie.AddedAtDateTime != default);

        Assert.True(addedMovie.ChangedAtDateTime == default);
        Assert.True(addedMovie.Genres.Count > 0);
        ////Assert.True(addedMovie.Reviews.Count > 0); //reviews are added later
        Assert.True(addedMovie.Actors.Count > 0);
        Assert.True(addedMovie.Budget > 0);
        Assert.True(addedMovie.ShortDescription.Length > 0);
        Assert.True(addedMovie.FullDescription.Length > 0);
        Assert.True(addedMovie.Year > 0);
        Assert.True(addedMovie.Title.Length > 0);
        Assert.True(addedMovie.Language.Length > 0);
    }


    [Trait("Category", "Service")]
    [Fact]
    public async Task UpdatingMovieUpdatesChangedAtDateTime()
    {
        using var ctx = CreateContext();
        var _movieService = MakeHelper.CreateMovieServiceWithRealMapper(ctx);

        _movieService.CreateAsync(new CreateMovieDto());
        var movieDto = new UpdateMovieDto();

        var movie = await ctx.Movies.FirstOrDefaultAsync();
        Assert.True(movie.ChangedAtDateTime == default);

        await _movieService.UpdateAsync(1, movieDto);

        await ctx.SaveChangesAsync();

        var updatedMovie = await ctx.Movies.FirstOrDefaultAsync();

        Assert.True(updatedMovie.ChangedAtDateTime != default);

    }

    // --- CreateAsync tests ---
    [Trait("Category", "Automapper")]
    [Fact]
    public void AutoMapper_Configuration_Is_Valid()
    {

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        }, NullLoggerFactory.Instance);

        config.AssertConfigurationIsValid();
    }



    [Fact]
    public async Task CreateAsync_CanAddMovieFromCreateMovieDto()
    {
        using var ctx = CreateContext();
        var service = MakeHelper.CreateMovieServiceWithRealMapper(ctx);
        var movieDto = new CreateMovieDto { Title = "Inception" };
        service.CreateAsync(movieDto);

        await ctx.SaveChangesAsync();

        Assert.True(ctx.Movies.Count() == 1);

        var movie = await ctx.Movies.FirstOrDefaultAsync();
        Assert.True(movie.Title == movieDto.Title);
    }

    /// <summary>
    /// Ensures creation fails when a movie with the same title already exists.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenTitleAlreadyExists_ReturnsFailure()
    {
        using var ctx = CreateContext();
        var createDto = new CreateMovieDto { Title = "Inception" };



        ctx.Movies.Add(new Movie { Title = "Inception", AddedAtDateTime = DateTime.UtcNow });
        await ctx.SaveChangesAsync();

        var result = await MakeHelper.CreateMovieServiceWithRealMapper(ctx)
            .CreateAsync(new CreateMovieDto { Title = "Inception" });

        Assert.False(result.Success);
        Assert.Contains("already exists", result.Error);
    }

    /// <summary>
    /// Ensures creation fails when provided genre IDs do not exist.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenGenreDoesNotExist_ReturnsFailure()
    {
        using var ctx = CreateContext();

        var result = await MakeHelper.CreateMovieServiceWithRealMapper(ctx).CreateAsync(new CreateMovieDto
        {
            Title = "New Movie",
            GenreIds = [999]
        });

        Assert.False(result.Success);
        Assert.Contains("genre", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Ensures valid movie creation returns success and correct mapped data.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsSuccessWithCorrectTitle()
    {
        using var ctx = CreateContext();
        var genre = new Genre { Name = "Action" };
        ctx.Genres.Add(genre);
        await ctx.SaveChangesAsync();
        var movieService = MakeHelper.CreateMovieServiceWithRealMapper(ctx);
      
        TestDataSeeder.Seed(ctx);
        var result = await movieService.CreateAsync(new CreateMovieDto
        {
            Title = "Top Gun",
            GenreIds = [genre.Id]
        });

        Assert.True(result.Success);
        Assert.Equal("Top Gun", result.Data!.Title);
    }

    /// <summary>
    /// Ensures AddedAt timestamp is set to UTC time during creation.
    /// </summary>
    [Fact]
    public async Task CreateAsync_SetsAddedAtToUtcNow()
    {
        using var ctx = CreateContext();
        var before = DateTime.UtcNow;

        await MakeHelper.CreateMovieServiceWithRealMapper(ctx).CreateAsync(new CreateMovieDto { Title = "Interstellar" });

        var movie = await ctx.Movies.FirstAsync();
        Assert.True(movie.AddedAtDateTime >= before);
    }

    // --- DeleteAsync tests ---

    /// <summary>
    /// Ensures delete fails when movie does not exist.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_WhenMovieDoesNotExist_ReturnsFailure()
    {
        using var ctx = CreateContext();

        var result = await MakeHelper.CreateMovieServiceWithRealMapper(ctx).DeleteAsync(999);

        Assert.False(result.Success);
        Assert.Equal("Movie not found", result.Error);
    }

    /// <summary>
    /// Ensures existing movie is deleted successfully.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_WhenMovieExists_ReturnsSuccess()
    {
        using var ctx = CreateContext();
        ctx.Movies.Add(new Movie { Title = "Heat", AddedAtDateTime = DateTime.UtcNow });
        await ctx.SaveChangesAsync();

        var movie = await ctx.Movies.FirstAsync();
        var result = await MakeHelper.CreateMovieServiceWithRealMapper(ctx).DeleteAsync(movie.Id);

        Assert.True(result.Success);
        Assert.Equal(0, await ctx.Movies.CountAsync());
    }

    // --- UpdateAsync tests ---

    /// <summary>
    /// Ensures update fails when movie does not exist.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WhenMovieDoesNotExist_ReturnsFailure()
    {
        using var ctx = CreateContext();

        var result = await MakeHelper.CreateMovieServiceWithRealMapper(ctx)
            .UpdateAsync(999, new UpdateMovieDto { Title = "Ghost" });

        Assert.False(result.Success);
        Assert.Equal("Movie not found", result.Error);
    }

    /// <summary>
    /// Ensures update fails when another movie already uses the requested title.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WhenTitleTakenByAnotherMovie_ReturnsFailure()
    {
        using var ctx = CreateContext();
        ctx.Movies.AddRange(
            new Movie { Title = "Taken", AddedAtDateTime = DateTime.UtcNow },
            new Movie { Title = "Heat", AddedAtDateTime = DateTime.UtcNow });

        await ctx.SaveChangesAsync();

        var heat = await ctx.Movies.FirstAsync(m => m.Title == "Heat");

        var result = await MakeHelper.CreateMovieServiceWithRealMapper(ctx)
            .UpdateAsync(heat.Id, new UpdateMovieDto { Title = "Taken" });

        Assert.False(result.Success);
        Assert.Contains("already exists", result.Error);
    }
    /// <summary>
    /// Creates a fresh in-memory EF Core DbContext for each test to ensure isolation.
    /// </summary>



    private static MovieDbContext CreateContext() =>
            new(new DbContextOptionsBuilder<MovieDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                // UseInMemoryDatabase Configures EF Core to use an in-memory database for testing purposes.
                // Each test gets a unique database instance (via Guid) to ensure isolation
                // and prevent data leaking between tests.
                .Options);


}
