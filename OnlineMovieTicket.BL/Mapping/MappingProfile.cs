using AutoMapper;
using OnlineMovieTicket.BL.DTOs.Banner;
using OnlineMovieTicket.BL.DTOs.Cinema;
using OnlineMovieTicket.BL.DTOs.City;
using OnlineMovieTicket.BL.DTOs.Country;
using OnlineMovieTicket.BL.DTOs.Movie;
using OnlineMovieTicket.BL.DTOs.Room;
using OnlineMovieTicket.BL.DTOs.Seat;
using OnlineMovieTicket.BL.DTOs.SeatType;
using OnlineMovieTicket.BL.DTOs.Showtime;
using OnlineMovieTicket.BL.DTOs.ShowtimeSeat;
using OnlineMovieTicket.BL.DTOs.User;
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

            CreateMap<Showtime, ShowtimeDTO>()
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.Room != null && src.Room.Cinema != null ? src.Room.Cinema.CityId : (long?)null))
                .ForMember(dest => dest.CinemaId, opt => opt.MapFrom(src => src.Room != null ? src.Room.CinemaId : (long?)null))
                .ForMember(dest => dest.CinemaName, opt => opt.MapFrom(src => src.Room != null ? src.Room.Cinema.Name : string.Empty))
                .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Room != null ? src.Room.Name : string.Empty))
                .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie != null ? src.Movie.Title : string.Empty));

            CreateMap<ShowtimeDTO,Showtime>()
                .ForMember(dest => dest.Movie, opt => opt.Ignore())
                .ForMember(dest => dest.Room, opt => opt.Ignore());

            CreateMap<ShowtimeSeat, ShowtimeSeatDTO>()
                .ForMember(dest => dest.PriceMultiplier, opt => opt.MapFrom(src => src.Seat != null && src.Seat.SeatType != null ? src.Seat.SeatType.PriceMultiplier : (decimal?)null))
                .ForMember(dest => dest.color, opt => opt.MapFrom(src => src.Seat != null && src.Seat.SeatType != null ? src.Seat.SeatType.Color : string.Empty))
                .ForMember(dest => dest.RowIndex, opt => opt.MapFrom(src => src.Seat != null ? src.Seat.RowIndex : (int?)null))
                .ForMember(dest => dest.ColumnIndex, opt => opt.MapFrom(src => src.Seat != null ? src.Seat.ColumnIndex : (int?)null));

            CreateMap<ShowtimeSeatDTO, ShowtimeSeat>()
                .ForMember(dest => dest.Seat, opt => opt.Ignore())
                .ForMember(dest => dest.Showtime, opt => opt.Ignore());
            
            CreateMap<ApplicationUser, UserDTO>()
                .ForMember(dest => dest.IsLockedOut, opt => opt.MapFrom(src => src.LockoutEnd.HasValue && src.LockoutEnd.Value > DateTimeOffset.UtcNow));

            CreateMap<UserDTO, ApplicationUser>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
                .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore())
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore());
            
            CreateMap<RoomWithShowtimes, RoomWithShowtimesDTO>()
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room))
                .ForMember(dest => dest.Cinema, opt => opt.MapFrom(src => src.Cinema))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Showtimes, opt => opt.MapFrom(src => src.Showtimes));

            CreateMap<ShowtimeQueryModel, ShowtimeQueryModelDTO>();
        }
    }
}
