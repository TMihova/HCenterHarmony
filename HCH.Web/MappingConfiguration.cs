using HCH.Models;
using HCH.Web.Models;
using System.Globalization;
using System.Linq;

namespace HCH.Web
{
    public class MappingConfiguration : AutoMapper.Profile
    {
        public MappingConfiguration()
        {
            CreateMap<HCHWebUser, UserViewModel>()
                .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => src.Profile.Name));

            CreateMap<FoodSupplement, FoodSupplementViewModel>().ReverseMap();

            CreateMap<OrderFoodSupplement, OrderProductViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.FoodSupplement.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FoodSupplement.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.FoodSupplement.Price))
                .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.ProductCount));

            CreateMap<Order, OrderViewModel>()
                .ForMember(dest => dest.OrderDate, 
                           opt => opt.MapFrom(src => src.OrderDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.Price, 
                           opt => opt.MapFrom(src => src.FoodSupplements.Sum(x => x.ProductCount * x.FoodSupplement.Price)))
                .ForMember(dest => dest.ClientFullName,
                           opt => opt.MapFrom(src => src.Client.FirstName + " " + src.Client.LastName));
        }
    }
}
