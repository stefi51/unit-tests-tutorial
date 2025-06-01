using AutoMapper;
using Template.Business.DTOs;
using Template.Domain;

namespace Template.Business;

public class BusinessMapperProfileUnit : Profile
{
    public BusinessMapperProfileUnit()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.UserUid, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.SurName))
            .ReverseMap();
        
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SurName, opt => opt.MapFrom(x => x.LastName));
        
    }
}