using Calender.Persistent.DatabaseContexts;
using Calender.Persistent.Entities;
using Microsoft.EntityFrameworkCore;

namespace Calender.Tests.TestHelpers
{
    internal sealed class DatabaseContextBuilder
    {
        private readonly List<Appointment> _appointments;

        public DatabaseContextBuilder()
        {
            this._appointments = new List<Appointment>();
        }

        public DatabaseContext Build()
        {
            var dbContext = new DatabaseContext(new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase("TestAppointmentsDatabase").Options);
            dbContext.Database.EnsureCreated();

            dbContext.Appointments.AddRange(this._appointments);

            dbContext.SaveChanges();
            return dbContext;
        }

        public DatabaseContextBuilder WithAppointment(DateTime startTime, DateTime endTime, bool isAttended, bool isDeleted)
        {
            var appointment = new Appointment
            {
                StartTime = startTime,
                EndTime = endTime,
                IsAttended = isAttended,
                IsDeleted = isDeleted
            };

            this._appointments.Add(appointment);
            return this;
        }
    }
}
