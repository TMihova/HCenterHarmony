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

            CreateMap<Appointment, AppointmentViewModel>()
                .ForMember(dest => dest.PatientFullName,
                           opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName))
                .ForMember(dest => dest.TherapistFullName,
                           opt => opt.MapFrom(src => src.Therapist.FirstName + " " + src.Therapist.LastName));

            CreateMap<AppointmentViewModel, Appointment>()
                .ForMember(dest => dest.Patient,
                           opt => opt.Ignore())
                .ForMember(dest => dest.Therapist,
                           opt => opt.Ignore());

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

            CreateMap<TherapyTreatmentViewModel, TherapyTreatment>()
                .ForMember(dest => dest.Therapy,
                           opt => opt.Ignore())
                .ForMember(dest => dest.Treatment,
                           opt => opt.Ignore());

            CreateMap<Therapy, TherapyViewModel>()
                .ForMember(dest => dest.TherapyId,
                           opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Patient,
                           opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName))
                .ForMember(dest => dest.Therapist,
                           opt => opt.MapFrom(src => src.Therapist.FirstName + " " + src.Therapist.LastName))
                 .ForMember(dest => dest.Treatments,
                           opt => opt.MapFrom(src => src.Treatments.Select(x => x.Treatment)));

            CreateMap<TherapyViewModel, Therapy>()
                .ForMember(dest => dest.Id,
                           opt => opt.MapFrom(src => src.TherapyId))
                .ForMember(dest => dest.Patient,
                           opt => opt.Ignore())
                .ForMember(dest => dest.Therapist,
                           opt => opt.Ignore())
                 .ForMember(dest => dest.Treatments,
                           opt => opt.MapFrom(src => src.Treatments.Where(x => x.Selected == true)));

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

            CreateMap<DeliveryNote, DeliveryNoteViewModel>()
                .ForMember(dest => dest.OrderDate,
                           opt => opt.MapFrom(src => src.Order.OrderDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.IssueDate,
                           opt => opt.MapFrom(src => src.IssueDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)));
        }
    }
}
