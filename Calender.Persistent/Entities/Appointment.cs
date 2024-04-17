namespace Calender.Persistent.Entities
{
    public sealed class Appointment
    {
        public int Id { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool? IsAttended { get; set; }

        public bool IsDeleted { get; set; }

        // public Status Status { get; set; }
    }
}