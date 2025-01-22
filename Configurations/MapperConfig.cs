
using AutoMapper;
using dotnet_notely.Data;
using dotnet_notely.ModelDtos;

namespace dotnet_notely.Configurations;

public class MapperConfig: Profile
{
    public MapperConfig()
    {
        CreateMap<RegisterDto, ApiUser>().ReverseMap();
    }
}