using Calender.Persistent.Repositories;
using Moq;

namespace Calender.Tests.Services
{
    [TestClass]
    public class AppointmentServiceTests
    {
        private readonly Mock<IAppointmentRepository> _repository;
        
        private readonly TimeSpan _workHoursStartTimeSpan;
        private readonly TimeSpan _workHoursEndTimeSpan;
        private readonly TimeSpan _reservedHoursStartTimeSpan;
        private readonly TimeSpan _reservedHoursEndTimeSpan;

        public AppointmentServiceTests()
        {
            _repository = new();
            _workHoursStartTimeSpan = new();
            _workHoursEndTimeSpan = new();
            _reservedHoursStartTimeSpan = new();
            _reservedHoursEndTimeSpan = new();
        }


    }
}
