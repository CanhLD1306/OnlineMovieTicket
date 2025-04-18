using AutoMapper;
using OnlineMovieTicket.BL.DTOs.Banner;
using OnlineMovieTicket.BL.DTOs.Cinema;
using OnlineMovieTicket.BL.DTOs.City;
using OnlineMovieTicket.BL.DTOs.Country;
using OnlineMovieTicket.BL.DTOs.Movie;
using OnlineMovieTicket.BL.DTOs.Room;
using OnlineMovieTicket.BL.DTOs.Seat;
using OnlineMovieTicket.BL.DTOs.SeatType;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Country, CountryDTO>().ReverseMap();
            CreateMap<SeatType, SeatTypeDTO>().ReverseMap();
            CreateMap<Banner,BannerDTO>().ReverseMap();
            CreateMap<Movie, MovieDTO>().ReverseMap();

            CreateMap<City, CityDTO>()
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country != null ? src.Country.Name : null));

            CreateMap<CityDTO, City>()
                .ForMember(dest => dest.Country, opt => opt.Ignore());

            CreateMap<Cinema, CinemaDTO>()
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City != null ? src.City.Name : string.Empty))
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.City != null && src.City.Country != null ? src.City.Country.Name : string.Empty))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.City != null ? src.City.CountryId : (long?)null));
            
            CreateMap<CinemaDTO, Cinema>()
                .ForMember(dest => dest.City, opt => opt.Ignore());

            CreateMap<Room, RoomDTO>()
                .ForMember(dest => dest.CinemaName, opt => opt.MapFrom(src => src.Cinema != null ? src.Cinema.Name : string.Empty))
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.Cinema != null && src.Cinema.City != null ? src.Cinema.City.Id : (long?)null))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.Cinema != null && src.Cinema.City != null ? src.Cinema.City.Name : string.Empty))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.Cinema != null && src.Cinema.City != null && src.Cinema.City.Country != null ? src.Cinema.City.Country.Id : (long?)null))
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Cinema != null && src.Cinema.City != null && src.Cinema.City.Country != null ? src.Cinema.City.Country.Name : string.Empty));

            CreateMap<RoomDTO, Room>()
                .ForMember(dest => dest.Cinema, opt => opt.Ignore());
            
            CreateMap<Seat, SeatDTO>()
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.SeatType != null ? src.SeatType.Color : string.Empty));
            
            CreateMap<SeatDTO, Seat>()
                .ForMember(dest => dest.SeatType, opt => opt.Ignore())
                .ForMember(dest => dest.Room, opt => opt.Ignore());
        }
    }
}
