namespace Calender.Business.Models
{
    public sealed class AppointmentModel
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool? IsAttended { get; set; }

        public bool IsDeleted { get; set; }

        // public Status Status { get; set; }
    }
}