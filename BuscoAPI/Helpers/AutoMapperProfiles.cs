using AutoMapper;
using BuscoAPI.DTOS;
using BuscoAPI.DTOS.Users;
using BuscoAPI.DTOS.Worker;
using BuscoAPI.Entities;

namespace BuscoAPI.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles() {
            CreateMap<UserPutDto, User>()
                .ForMember(x => x.Image, options => options.Ignore());

            CreateMap<User, UserDTO>()
                .ForMember(x => x.Worker, opt => opt.MapFrom(src => src.Worker));

            //De WorkerCreationDTO a Worker
            CreateMap<WorkerCreationDTO, Worker>();

            CreateMap<Worker, WorkerDTO>()
                .ForMember(x => x.WorkersProfessions, opt => opt.MapFrom(src => src.WorkersProfessions));

            CreateMap<WorkersProfessions, WorkersProfessionsDTO>()
                .ForMember(x => x.Profession, opt => opt.MapFrom(src => src.Profession));
        }
    }
}
