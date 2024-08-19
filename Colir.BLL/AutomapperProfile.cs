using AutoMapper;
using Colir.BLL.Models;
using DAL.Entities;

namespace Colir.BLL;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<Attachment, AttachmentModel>();

        CreateMap<User, UserModel>();

        CreateMap<User, DetailedUserModel>();

        CreateMap<Message, MessageModel>()
            .ForMember(dest => dest.AuthorHexId,
                opt => opt.MapFrom(src => src.Author!.HexId))
            .ForMember(dest => dest.RoomGuid,
                opt => opt.MapFrom(src => src.Room.Guid));

        CreateMap<Reaction, ReactionModel>()
            .ForMember(dest => dest.AuthorHexId,
            opt => opt.MapFrom(src => src.Author.HexId));

        CreateMap<Room, RoomModel>();

        CreateMap<UserSettings, UserSettingsModel>()
            .ReverseMap();

        CreateMap<UserStatistics, UserStatisticsModel>()
            .ReverseMap();
    }

    public static MapperConfiguration InitializeAutoMapper()
    {
        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AutomapperProfile());
        });

        return config;
    }
}