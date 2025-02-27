using AutoMapper;
using CarsStore.Core.DTO;
using CarsStore.Core.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Car, CarDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
        CreateMap<User, UserDto>();
    }
}
