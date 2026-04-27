using AutoMapper;
using F1ApiWrapper.DTOs;
using F1ApiWrapper.Models;

namespace F1ApiWrapper.Profiles;

public class OpenF1MappingProfile : Profile
{
    public OpenF1MappingProfile()
    {
        CreateMap<OpenF1DriverResponse, Driver>()
            .ForMember(destination => destination.Team, options => options.MapFrom(source => source.TeamName));
    }
}
