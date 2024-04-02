using AutoMapper;
using CleanArchitecture.Identity.Application.Dtos;
using CleanArchitecture.Identity.Repository.Entities;

namespace CleanArchitecture.Identity.Application;

public sealed class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<UserCreationDto, User>();
        CreateMap<UserCreationDto, User>().ForMember(destination => destination.UserName,
            opts => opts.MapFrom(source => source.Name));
        CreateMap<User, UserDto>();
        CreateMap<User, UserDto>().ForMember(destination => destination.Name,
            opts => opts.MapFrom(source => source.UserName));
    }
}