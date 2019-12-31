using AutoMapper;

namespace CeloPracticalChallenge.API.Profiles
{
    public class RandomUsersProfile : Profile
    {
        public RandomUsersProfile()
        {
            CreateMap<DTOs.RandomUserForUpdateDto, Entities.RandomUser>()
              .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name.Title))
              .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name.Last))
              .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name.First));

            CreateMap<Entities.RandomUser, DTOs.RandomUserForUpdateDto>()
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => new DTOs.NameDto { Title = src.Title, Last = src.LastName, First = src.FirstName }));

            CreateMap<Entities.RandomUser, DTOs.RandomUserDto>()
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => new DTOs.NameDto { Title = src.Title, Last = src.LastName, First = src.FirstName }))
              .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.ThrumbnailURL));
        }
    }
}
