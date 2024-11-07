using AutoMapper;
using BuscoAPI.DTOS;
using BuscoAPI.DTOS.Chat;
using BuscoAPI.DTOS.Notification;
using BuscoAPI.DTOS.Proposals;
using BuscoAPI.DTOS.Users;
using BuscoAPI.DTOS.Worker;
using BuscoAPI.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace BuscoAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            CreateMap<UserPutDto, User>()
                .ForMember(x => x.Image, options => options.Ignore())
                .ForMember(x => x.Ubication, x => x.MapFrom(y =>
                    geometryFactory.CreatePoint(new Coordinate(y.Longitude, y.Latitude))));

            CreateMap<User, UserDTO>()
                .ForMember(x => x.Worker, opt => opt.MapFrom(src => src.Worker))
                .ForMember(x => x.Latitude, x => x.MapFrom(y => y.Ubication.Y))
                .ForMember(x => x.Longitude, x => x.MapFrom(y => y.Ubication.X));


            CreateMap<User, UserBasicDTO>()
                .ForMember(x => x.Worker, opt => opt.MapFrom(src => src.Worker))
                .ForMember(x => x.Latitude, x => x.MapFrom(y => y.Ubication.Y))
                .ForMember(x => x.Longitude, x => x.MapFrom(y => y.Ubication.X));

            CreateMap<User, UserWithoutWorker>()
                .ForMember(x => x.Latitude, x => x.MapFrom(y => y.Ubication.Y))
                .ForMember(x => x.Longitude, x => x.MapFrom(y => y.Ubication.X));

            CreateMap<WorkerCreationDTO, Worker>();

            CreateMap<Worker, WorkerDTO>()
                .ForMember(x => x.Professions, opt => opt.MapFrom(src => src.WorkersProfessions.Select(x => x.Profession)));

            CreateMap<Worker, WorkerWithoutUser>()
                .ForMember(x => x.Professions, opt => opt.MapFrom(src => src.WorkersProfessions.Select(x => x.Profession)));

            //CreateMap<Worker, WorkerWithQualification>()
            //    .ForMember(x => x.WorkersProfessions, opt => opt.MapFrom(src => src.WorkersProfessions));


            CreateMap<WorkersProfessions, WorkersProfessionsDTO>()
                .ForMember(x => x.Profession, opt => opt.MapFrom(src => src.Profession));

            CreateMap<ProposalCreationDTO, Proposal>()
                .ForMember(x => x.Image, opt => opt.Ignore())
                .ForMember(x => x.Ubication, x => x.MapFrom(y =>
                     geometryFactory.CreatePoint(new Coordinate(y.Longitude, y.Latitude))));

            CreateMap<Proposal, ProposalDTO>()
                .ForMember(x => x.Latitude, x => x.MapFrom(y => y.Ubication.Y))
                .ForMember(x => x.Longitude, x => x.MapFrom(y => y.Ubication.X));

            CreateMap<Application, ApplicationDTO>();

            CreateMap<Worker, WorkerApplicationDTO>();

            CreateMap<User, UserApplicationDTO>();

            CreateMap<QualificationCreationDTO, Qualification>().ReverseMap();

            CreateMap<Qualification, QualificationDTO>();

            CreateMap<Chat, ChatDTO>();


            CreateMap<Notification, NotificationCreationDTO>().ReverseMap();
            CreateMap<Notification, NotificationDTO>().ReverseMap();
        }
    }
}
