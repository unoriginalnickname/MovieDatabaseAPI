using AutoMapper;

namespace EFCorePracticeProject.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Movie
        CreateMap<Movie, MovieDto>();
        CreateMap<Movie, MovieDetailsDto>();

        CreateMap<UpdateMovieDto, Movie>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.AddedAtDateTime, opt => opt.Ignore())
            .ForMember(d => d.ChangedAtDateTime, opt => opt.Ignore())
            .ForMember(d => d.Actors, opt => opt.Ignore())
            .ForMember(d => d.Genres, opt => opt.Ignore())
            .ForMember(d => d.Reviews, opt => opt.Ignore());


        CreateMap<CreateMovieDto, Movie>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.AddedAtDateTime, opt => opt.Ignore())
            .ForMember(d => d.ChangedAtDateTime, opt => opt.Ignore())
            .ForMember(d => d.Actors, opt => opt.Ignore())
            .ForMember(d => d.Genres, opt => opt.Ignore())
            .ForMember(d => d.Reviews, opt => opt.Ignore());

        // Actor
        CreateMap<Actor, ActorDto>();
        CreateMap<CreateActorDto, Actor>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Movies, opt => opt.Ignore())
            .ForMember(d => d.BirthYear, opt => opt.MapFrom(s => s.BirthYear))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description));

        CreateMap<UpdateActorDto, Actor>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Movies, opt => opt.Ignore());
        
        // Genre
        CreateMap<Genre, GenreDto>();

        // Review
        CreateMap<Review, ReviewDto>();

        CreateMap<CreateReviewDto, Review>()
            .ForMember(d => d.MovieId, opt => opt.Ignore())
            .ForMember(d => d.Movie, opt => opt.Ignore())
            .ForMember(d => d.Id, opt => opt.Ignore());

    }
}
