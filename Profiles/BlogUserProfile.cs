using AutoMapper;
using Blog.Areas.Identity.Data;
using Blog.Models;
public class BlogUserProfile : Profile
{
    public BlogUserProfile()
    {
        CreateMap<UserProfile, BlogUser>()
            .ForMember(
                dest => dest.FirstName,
                opt => opt.MapFrom(src => $"{src.FirstName}")
            )
            .ForMember(
                dest => dest.LastName,
                opt => opt.MapFrom(src => $"{src.LastName}")
            )
            .ForMember(
                dest => dest.Email,
                opt => opt.MapFrom(src => $"{src.Email}")
            )
            .ForMember(
                dest => Convert.ToDateTime(dest.DOB),
                opt => opt.MapFrom(src => $"{src.DOB}")
            )
            .ForMember(
                dest => dest.Gender,
                opt => opt.MapFrom(src => $"{src.Gender}")
            )
            .ForMember(
                dest => dest.PlanType,
                opt => opt.MapFrom(src => $"{src.PlanType}")
            )
            .ForMember(
                dest => dest.AvatarPath,
                opt => opt.MapFrom(src => $"{src.AvatarPath}")
            );
    }
}