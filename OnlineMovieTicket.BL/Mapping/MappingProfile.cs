using AutoMapper;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.City;
using OnlineMovieTicket.BL.DTOs.Country;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Country, CountryDTO>().ReverseMap();

            CreateMap<City, CityDTO>().ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country != null ? src.Country.Name : null));

            CreateMap<CityDTO, City>().ForMember(dest => dest.Country, opt => opt.Ignore());
        }
    }
}
