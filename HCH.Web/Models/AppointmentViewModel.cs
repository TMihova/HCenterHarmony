using HCH.Models.Enums;

namespace HCH.Web.Models
{
    public class AppointmentViewModel
    {
        public DayOfWeekBg DayOfWeekBg { get; set; }

        public string VisitingHour { get; set; }

        public decimal Price { get; set; }

        public string Therapist { get; set; }
    }
}
