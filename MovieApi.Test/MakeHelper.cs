using AutoMapper;
using EFCorePracticeProject.Mappings;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MovieApi.Test
{
    public static class MakeHelper
    {
        ///// <summary>
        ///// Creates a MovieService instance with a real UnitOfWork and mocked AutoMapper.
        ///// </summary>
        ////public static MovieService CreateMovieServiceWithMockMapper(MovieDbContext ctx) =>
        ////    new(new UnitOfWork(ctx), mockMapper());

        /// <summary>
        /// Creates a MovieService instance with a real UnitOfWork and real AutoMapper.
        /// </summary>
        //public static MovieService CreateMovieServiceWithRealMapper(MovieDbContext ctx) =>
        //    new(new UnitOfWork(ctx), CreateRealMapper(),NullLoggerFactory.Instance);

        public static IMapper CreateRealMapper()
        {
            // Configure AutoMapper with your mapping profile + logger support
            var config =
                new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); }, NullLoggerFactory.Instance);

            // Validate all mappings at startup
            config.AssertConfigurationIsValid();

            // Return mapper instance
            return config.CreateMapper();
        }

        ///// <summary>
        ///// Builds a mocked AutoMapper instance to simulate mapping between DTOs and domain models.
        ///// </summary>
        //public static IMapper mockMapper() //Why not just use a regular automapper with mappings
        //{
        //    var mapperMock = new Mock<IMapper>();

        //    // It.IsAny<T>()
        //    // Means: "match any input of this type"
        //    // So this setup triggers for ANY CreateMovieDto, not a specific one
        //    mapperMock.Setup(m => m.Map<Movie>(It.IsAny<CreateMovieDto>()))

        //        // Returns(...)
        //        // Means: "when this method is called, return this value"
        //        // Here we manually construct what AutoMapper would have produced
        //        .Returns((CreateMovieDto src) => new Movie
        //        {
        //            Title = src.Title
        //        });

        //    // Same idea:
        //    // It.IsAny<UpdateMovieDto>() → match any UpdateMovieDto input
        //    mapperMock.Setup(m => m.Map<Movie>(It.IsAny<UpdateMovieDto>()))

        //        // Returns(...) again creates a new object as the result
        //        .Returns((UpdateMovieDto src) => new Movie
        //        {
        //            Title = src.Title
        //        });

        //    // Overload: Map(source, destination)
        //    mapperMock.Setup(m => m.Map(It.IsAny<UpdateMovieDto>(), It.IsAny<Movie>()))

        //        // Callback(...)
        //        // Means: "don’t return a value, instead execute this logic when called"
        //        // Used when method modifies an existing object instead of returning a new one
        //        .Callback((UpdateMovieDto src, Movie dest) =>
        //        {
        //            // simulate AutoMapper updating existing entity in-place
        //            dest.Title = src.Title;
        //        });

        //    // Map Movie → MovieDto
        //    mapperMock.Setup(m => m.Map<MovieDto>(It.IsAny<Movie>()))

        //        // Returns(...) creates the DTO that would normally be returned by AutoMapper
        //        .Returns((Movie src) => new MovieDto
        //        {
        //            Id = src.Id,
        //            Title = src.Title
        //        });

        //    // mock.Object = the real IMapper instance that uses these rules
        //    return mapperMock.Object;
        //}



        /// <summary>
        /// Makes a populated movie instance.
        /// </summary>
        public static CreateMovieDto MakeCreateMovieDto()
        {
            return new CreateMovieDto()
            {
                Title = "Neon Horizon",
                Year = 2026,
                ShortDescription = "A rogue hacker uncovers a hidden AI controlling global systems.",
                FullDescription =
                    "In a near-future cyberpunk world, a disillusioned hacker stumbles upon evidence that a hidden artificial intelligence has been subtly steering governments and corporations for decades. As he digs deeper, he becomes the target of a global surveillance network. Forced to trust a small group of outcasts, he must decide whether to expose the truth or merge with the system to survive.",
                Language = "English",
                Duration = "2h 18m",
                Budget = 85000000,

                GenreIds = new List<int>() { 1, 2 },
                ActorIds = new List<int>() { 1, 2 }

            };
        }
    }
}
