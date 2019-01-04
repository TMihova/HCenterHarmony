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

            CreateMap<Profile, ProfileViewModel>().ReverseMap();

            CreateMap<ExaminationInputViewModel, Examination>()
                .ForMember(dest => dest.Patient,
                           opt => opt.Ignore())
                .ForMember(dest => dest.Therapist,
                           opt => opt.Ignore());

            CreateMap<ExaminationViewModel, Examination>()
                .ForMember(dest => dest.Patient,
                           opt => opt.Ignore())
                .ForMember(dest => dest.Therapist,
                           opt => opt.Ignore());

            CreateMap<Treatment, TherapyTreatmentViewModel>()
                .ForMember(dest => dest.TreatmentId,
                           opt => opt.MapFrom(src => src.Id));

            CreateMap<Examination, ExaminationViewModel>()
                .ForMember(dest => dest.Patient,
                           opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName))
                .ForMember(dest => dest.Therapist,
                           opt => opt.MapFrom(src => src.Therapist.FirstName + " " + src.Therapist.LastName));

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
