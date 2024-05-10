using AutoMapper;
using BuscoAPI.DTOS.Users;
using BuscoAPI.Entities;

namespace BuscoAPI.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles() {
            CreateMap<UserPutDto, User>()
                .ForMember(x => x.Image, options => options.Ignore());
        }
    }
}
