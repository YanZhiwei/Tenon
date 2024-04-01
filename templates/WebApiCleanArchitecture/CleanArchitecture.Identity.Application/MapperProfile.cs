using AutoMapper;
using CleanArchitecture.Identity.Application.Dtos;
using CleanArchitecture.Identity.Repository.Entities;

namespace CleanArchitecture.Identity.Application;

public sealed class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserCreationDto, User>();
    }
}