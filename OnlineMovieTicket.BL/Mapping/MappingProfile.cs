using AutoMapper;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Country;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Country, CountryDTO>().ReverseMap();
        }
    }
}
