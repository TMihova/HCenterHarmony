namespace HCH.Models
{
    public class TherapyTreatment
    {
        public string TherapyId { get; set; }

        public virtual Therapy Therapy { get; set; }

        public string TreatmentId { get; set; }

        public virtual Treatment Treatment { get; set; }
    }
}