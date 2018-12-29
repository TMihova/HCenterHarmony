using HCH.Models;
using HCH.Web.Models;

namespace HCH.Web
{
    public class MappingConfiguration : AutoMapper.Profile
    {
        public MappingConfiguration()
        {
            CreateMap<HCHWebUser, UserViewModel>()
                .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => src.Profile.Name));
        }
    }
}
