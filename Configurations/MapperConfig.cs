
using AutoMapper;
using dotnet_notely.Data;
using dotnet_notely.ModelDtos;
using dotnet_notely.ModelDtos.NoteDtos;

namespace dotnet_notely.Configurations;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<RegisterDto, ApiUser>().ReverseMap();
        CreateMap<CreateNoteDto, Note>().ReverseMap();
        CreateMap<Note, NoteResponseDto>();
        CreateMap<CreateNoteDto, NoteResponseDto>();
    }
}