using Calender.Business.Models;
using Calender.Business.Services;
using Calender.Persistent.Entities;
using Calender.Persistent.Repositories;
using Moq;

namespace Calender.Tests.Services
{
    [TestClass]
    public class AppointmentServiceTests
    {
        private readonly Mock<IAppointmentRepository> _appointmentRepository;

        private readonly TimeSpan _workHoursStartTimeSpan;
        private readonly TimeSpan _workHoursEndTimeSpan;
        private readonly TimeSpan _reservedHoursStartTimeSpan;
        private readonly TimeSpan _reservedHoursEndTimeSpan;

        public AppointmentServiceTests()
        {
            _appointmentRepository = new();

            _workHoursStartTimeSpan = new TimeSpan(09, 00, 00);
            _workHoursEndTimeSpan = new TimeSpan(17, 00, 00);
            _reservedHoursStartTimeSpan = new TimeSpan(16, 00, 00);
            _reservedHoursEndTimeSpan = new TimeSpan(17, 00, 00);
        }

        [TestMethod]
        public async Task FindAvailableTimeslots_ThrowsException_WhenDateIsNull()
        {
            // Arrange
            IAppointmentService appointmentService = new AppointmentService(
                new Mock<IAppointmentRepository>().Object,
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>());

            // Act

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await appointmentService.FindAvailableTimeslots(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FindAvailableTimeslots_ReturnsAllAvailableTimeSlots_WhenNoAppointmentIsBooked()
        {
            // Arrange
            _appointmentRepository
                .Setup(repo => repo.ListAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Appointment>());

            IAppointmentService appointmentService = new AppointmentService(
                _appointmentRepository.Object,
                _workHoursStartTimeSpan,
                _workHoursEndTimeSpan,
                _reservedHoursStartTimeSpan,
                _reservedHoursEndTimeSpan);

            // Act
            var result = await appointmentService.FindAvailableTimeslots(new DateTime(2024, 01, 05).Date, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(16, result.Count);
    }

        [TestMethod]
        public async Task FindAvailableTimeslots_ReturnsFewAvailableTimeSlots_WhenSomeAppointmentsAreBooked()
        {
            // Arrange
            DateTime startDateTime = new(2024, 01, 10, 10, 00, 00);
            DateTime endDateTime = new(2024, 01, 10, 10, 30, 00);

            _appointmentRepository
                .Setup(repo => repo.ListAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Appointment>() {
                    new() {
                        Id = 1,
                        StartTime = startDateTime,
                        EndTime = endDateTime
                    },
                    new() {
                        Id = 2,
                        StartTime = startDateTime.AddHours(1),
                        EndTime = endDateTime.AddHours(1)
                    }
                });

            IAppointmentService appointmentService = new AppointmentService(
                _appointmentRepository.Object,
                _workHoursStartTimeSpan,
                _workHoursEndTimeSpan,
                _reservedHoursStartTimeSpan,
                _reservedHoursEndTimeSpan);

            // Act
            var result = await appointmentService.FindAvailableTimeslots(new DateTime(2024, 01, 10).Date, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(14, result.Count);
        }

        [TestMethod]
        public async Task FindAvailableTimeslots_ReturnsAllTimeSlots_BetweenWorkHours()
        {
            // Arrange
            _appointmentRepository
                .Setup(repo => repo.ListAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Appointment>());

            IAppointmentService appointmentService = new AppointmentService(
                _appointmentRepository.Object,
                _workHoursStartTimeSpan,
                _workHoursEndTimeSpan,
                _reservedHoursStartTimeSpan,
                _reservedHoursEndTimeSpan);

            // Act
            var result = await appointmentService.FindAvailableTimeslots(new DateTime(2024, 01, 05).Date, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);

            var firstTimeSlot = result.First();
            var lastTimeSlot = result.Last();
            Assert.IsTrue(firstTimeSlot.StartTime.Hour == _workHoursStartTimeSpan.Hours);
            Assert.IsTrue(lastTimeSlot.EndTime.Hour == _workHoursEndTimeSpan.Hours);
        }

        [TestMethod]
        public async Task AddAppointment_ThrowsException_WhenAppointmentIsNull()
        {
            // Arrange
            IAppointmentService appointmentService = new AppointmentService(
                new Mock<IAppointmentRepository>().Object,
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>());

            // Act

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await appointmentService.AddAppointment(It.IsAny<AppointmentModel>(), It.IsAny<CancellationToken>()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task AddAppointment_ThrowsException_WhenAppointmentOnTheRequestedDateAndTimeAlreadyExists()
        {
            // Arrange
            DateTime startDateTime = new(2024, 01, 10, 10, 00, 00);
            DateTime endDateTime = new(2024, 01, 10, 10, 30, 00);

            var appointmentModel = new AppointmentModel
            {
                StartTime = startDateTime,
                EndTime = endDateTime
            };

            _appointmentRepository
                .Setup(repo => repo.GetAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Appointment
                {
                    Id = 1,
                    StartTime = startDateTime,
                    EndTime = endDateTime
                });

            IAppointmentService appointmentService = new AppointmentService(
                _appointmentRepository.Object,
                _workHoursStartTimeSpan,
                _workHoursEndTimeSpan,
                _reservedHoursStartTimeSpan,
                _reservedHoursEndTimeSpan);

            // Act

            // Assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(
                async() =>
                    await appointmentService.AddAppointment(appointmentModel, It.IsAny<CancellationToken>()).ConfigureAwait(false))
                .ConfigureAwait(false);

            Assert.IsInstanceOfType(exception, typeof(ArgumentException));
            Assert.AreEqual(exception.Message, $"Appointment on the given date & time {startDateTime} already exists!");
        }

        [TestMethod]
        public async Task AddAppointment_ReturnsNull_WhenRequestedAppointmentTimeIsNotWithinWorkingHours()
        {
            // Arrange
            var appointmentModel = new AppointmentModel
            {
                StartTime = new DateTime(2024, 01, 10, 18, 00, 00),
                EndTime = new DateTime(2024, 01, 10, 18, 30, 00)
            };

            _appointmentRepository
                .Setup(repo => repo.GetAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<Appointment>());

            IAppointmentService appointmentService = new AppointmentService(
                _appointmentRepository.Object,
                _workHoursStartTimeSpan,
                _workHoursEndTimeSpan,
                _reservedHoursStartTimeSpan,
                _reservedHoursEndTimeSpan);

            // Act
            var result = await appointmentService.AddAppointment(appointmentModel, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AddAppointment_ReturnsNull_WhenRequestedAppointmentTimeIsWithinReservedHours()
        {
            // Arrange
            var appointmentModel = new AppointmentModel
            {
                StartTime = new DateTime(2024, 04, 16, 16, 00, 00),
                EndTime = new DateTime(2024, 04, 16, 16, 30, 00)
            };

            _appointmentRepository
                .Setup(repo => repo.GetAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<Appointment>());

            IAppointmentService appointmentService = new AppointmentService(
                _appointmentRepository.Object,
                _workHoursStartTimeSpan,
                _workHoursEndTimeSpan,
                _reservedHoursStartTimeSpan,
                _reservedHoursEndTimeSpan);

            // Act
            var result = await appointmentService.AddAppointment(appointmentModel, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AddAppointment_ReturnsAppointment_WhenRequestedAppointmentTimeIsWithinWorkingHoursAndNotInReservedHours()
        {
            // Arrange
            var appointmentModel = new AppointmentModel
            {
                StartTime = new DateTime(2024, 04, 12, 15, 00, 00),
                EndTime = new DateTime(2024, 04, 12, 15, 30, 00)
            };

            _appointmentRepository
                .Setup(repo => repo.GetAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<Appointment>());

            _appointmentRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Appointment { StartTime = appointmentModel.StartTime, EndTime = appointmentModel.EndTime });

            IAppointmentService appointmentService = new AppointmentService(
                _appointmentRepository.Object,
                _workHoursStartTimeSpan,
                _workHoursEndTimeSpan,
                _reservedHoursStartTimeSpan,
                _reservedHoursEndTimeSpan);

            // Act
            var result = await appointmentService.AddAppointment(appointmentModel, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(appointmentModel.StartTime, result.StartTime);
            Assert.AreEqual(appointmentModel.EndTime, result.EndTime);
        }

        [TestMethod]
        public async Task KeepAppointment_ThrowsException_WhenStartTimeIsNull()
        {
            // Arrange
            IAppointmentService appointmentService = new AppointmentService(
                new Mock<IAppointmentRepository>().Object,
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>());

            // Act

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await appointmentService.KeepAppointment(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task KeepAppointment_ThrowsException_WhenAppointmentOnRequestedDateAndTimeDoesNotExist()
        {
            // Arrange
            DateTime startTime = DateTime.Today.AddHours(10);

            _appointmentRepository
                .Setup(repo => repo.GetAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<Appointment>());

            IAppointmentService appointmentService = new AppointmentService(
                _appointmentRepository.Object,
                _workHoursStartTimeSpan,
                _workHoursEndTimeSpan,
                _reservedHoursStartTimeSpan,
                _reservedHoursEndTimeSpan);

            // Act

            // Assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () =>
                    await appointmentService.KeepAppointment(startTime, It.IsAny<CancellationToken>()).ConfigureAwait(false))
                .ConfigureAwait(false);

            Assert.IsInstanceOfType(exception, typeof(ArgumentException));
            Assert.AreEqual(exception.Message, $"Appointment for the given date & time {startTime} doesn't exist!");
        }

        [TestMethod]
        public async Task KeepAppointment_ReturnsAppointmentWithMarkedAsAttended_WhenAppointmentOnRequestedDateAndTimeExists()
        {
            // Arrange
            DateTime startTime = new(2024, 04, 12, 15, 00, 00);
            DateTime endTime = new(2024, 04, 12, 15, 30, 00);

            Appointment appointment = new() { StartTime = startTime, EndTime = endTime };

            _appointmentRepository
                .Setup(repo => repo.GetAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _appointmentRepository
                .Setup(repo => repo.UpdateAsync(appointment, It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new Appointment {
                        StartTime = startTime,
                        EndTime = endTime,
                        IsAttended = true
                    });

            IAppointmentService appointmentService = new AppointmentService(
                _appointmentRepository.Object,
                _workHoursStartTimeSpan,
                _workHoursEndTimeSpan,
                _reservedHoursStartTimeSpan,
                _reservedHoursEndTimeSpan);

            // Act
            var result = await appointmentService.KeepAppointment(startTime, It.IsAny<CancellationToken>() ).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(startTime, result.StartTime);
            Assert.AreEqual(endTime, result.EndTime);
            Assert.AreEqual(true, result.IsAttended);
        }

        [TestMethod]
        public async Task DeleteAppointment_ThrowsException_WhenStartTimeIsNull()
        {
            // Arrange
            IAppointmentService appointmentService = new AppointmentService(
                new Mock<IAppointmentRepository>().Object,
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>());

            // Act

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await appointmentService.DeleteAppointment(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteAppointment_ThrowsException_WhenAppointmentOnRequestedDateAndTimeDoesNotExist()
        {
            // Arrange
            DateTime startTime = DateTime.Today.AddHours(15);

            _appointmentRepository
                .Setup(repo => repo.GetAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<Appointment>());

            IAppointmentService appointmentService = new AppointmentService(
                _appointmentRepository.Object,
                _workHoursStartTimeSpan,
                _workHoursEndTimeSpan,
                _reservedHoursStartTimeSpan,
                _reservedHoursEndTimeSpan);

            // Act

            // Assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () =>
                    await appointmentService.DeleteAppointment(startTime, It.IsAny<CancellationToken>()).ConfigureAwait(false))
                .ConfigureAwait(false);

            Assert.IsInstanceOfType(exception, typeof(ArgumentException));
            Assert.AreEqual(exception.Message, $"Appointment for the given date & time {startTime} doesn't exist!");
        }

        [TestMethod]
        public async Task DeleteAppointment_DeletesAppointmentAndReturnsTrue_WhenAppointmentOnRequestedDateAndTimeExists()
        {
            // Arrange
            DateTime startTime = new(2024, 03, 28, 13, 00, 00);
            DateTime endTime = new(2024, 03, 28, 13, 30, 00);

            Appointment appointment = new() { StartTime = startTime, EndTime = endTime };

            _appointmentRepository
                .Setup(repo => repo.GetAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _appointmentRepository
                .Setup(repo => repo.DeleteAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            IAppointmentService appointmentService = new AppointmentService(
                _appointmentRepository.Object,
                _workHoursStartTimeSpan,
                _workHoursEndTimeSpan,
                _reservedHoursStartTimeSpan,
                _reservedHoursEndTimeSpan);

            // Act
            var result = await appointmentService.DeleteAppointment(startTime, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
