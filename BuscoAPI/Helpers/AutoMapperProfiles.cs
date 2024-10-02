using AutoMapper;
using BuscoAPI.DTOS;
using BuscoAPI.DTOS.Chat;
using BuscoAPI.DTOS.Notification;
using BuscoAPI.DTOS.Proposals;
using BuscoAPI.DTOS.Users;
using BuscoAPI.DTOS.Worker;
using BuscoAPI.Entities;

namespace BuscoAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserPutDto, User>()
                .ForMember(x => x.Image, options => options.Ignore());

            CreateMap<User, UserDTO>()
                .ForMember(x => x.Worker, opt => opt.MapFrom(src => src.Worker));

            CreateMap<User, UserBasicDTO>()
                .ForMember(x => x.Worker, opt => opt.MapFrom(src => src.Worker));

            //De WorkerCreationDTO a Worker
            CreateMap<WorkerCreationDTO, Worker>();

            CreateMap<Worker, WorkerDTO>()
                .ForMember(x => x.WorkersProfessions, opt => opt.MapFrom(src => src.WorkersProfessions));
            
            CreateMap<Worker, WorkerWithoutUser>()
                .ForMember(x => x.WorkersProfessions, opt => opt.MapFrom(src => src.WorkersProfessions));
            
            CreateMap<Worker, WorkerWithQualification>()
                .ForMember(x => x.WorkersProfessions, opt => opt.MapFrom(src => src.WorkersProfessions));


            CreateMap<WorkersProfessions, WorkersProfessionsDTO>()
                .ForMember(x => x.Profession, opt => opt.MapFrom(src => src.Profession));

            CreateMap<ProposalCreationDTO, Proposal>()
                .ForMember(x => x.Image, opt => opt.Ignore());

            CreateMap<Proposal, ProposalDTO>();
            
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
