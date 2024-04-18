using Calender.Persistent.Entities;
using Calender.Persistent.Repositories;
using Calender.Tests.TestHelpers;
using Moq;

namespace Calender.Tests.Repositories
{
    [TestClass]
    public class AppointmentRepositoryTests
    {
        private IAppointmentRepository _appointmentRepository;

        [TestMethod]
        public async Task ListAsync_ThrowsException_WhenDateIsNull()
        {
            // Arrange
            this._appointmentRepository = new AppointmentRepository(new DatabaseContextBuilder().Build());

            // Act

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await _appointmentRepository.ListAsync(default, It.IsAny<CancellationToken>()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ListAsync_ReturnsAllAppointmentsForGivenDate_WhenDateIsGiven()
        {
            // Arrange
            DateTime startDateTime = new DateTime(2024, 04, 06, 10, 00, 00);
            DateTime endDateTime = new DateTime(2024, 04, 09, 10, 30, 00);

            this._appointmentRepository = new AppointmentRepository(
                new DatabaseContextBuilder().WithAppointment(
                    startDateTime,
                    endDateTime,
                    false,
                    false)
                .Build());

            // Act
            List<Appointment> appointments = await this._appointmentRepository.ListAsync(startDateTime.Date, It.IsAny<CancellationToken>()).ConfigureAwait(!false);

            // Assert
            Assert.IsNotNull(appointments);
            Assert.AreEqual(1, appointments.Count());
            Assert.AreEqual(startDateTime, appointments[0].StartTime);
            Assert.AreEqual(endDateTime, appointments[0].EndTime);
        }

        [TestMethod]
        public async Task GetAsync_ThrowsException_WhenDateIsNull()
        {
            // Arrange
            this._appointmentRepository = new AppointmentRepository(new DatabaseContextBuilder().Build());

            // Act

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await _appointmentRepository.GetAsync(default, It.IsAny<CancellationToken>()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetAsync_ReturnsNullForGivenDateAndTime_WhenAppointmentDoesNotExist()
        {
            // Arrange
            DateTime startDateTime = new DateTime(2024, 04, 12, 12, 00, 00);
            DateTime endDateTime = new DateTime(2024, 04, 12, 12, 30, 00);

            this._appointmentRepository = new AppointmentRepository(
                new DatabaseContextBuilder().WithAppointment(
                    startDateTime,
                    endDateTime,
                    false,
                    false)
                .Build());

            // Act
            Appointment? appointment = await this._appointmentRepository.GetAsync(DateTime.Now.AddDays(10), It.IsAny<CancellationToken>()).ConfigureAwait(!false);

            // Assert
            Assert.IsNull(appointment);
        }

        [TestMethod]
        public async Task GetAsync_ReturnsOneAppointmentForGivenDateAndTime_WhenAppointmentExists()
        {
            // Arrange
            DateTime startDateTime = new DateTime(2024, 04, 15, 09, 00, 00);
            DateTime endDateTime = new DateTime(2024, 04, 15, 09, 30, 00);

            this._appointmentRepository = new AppointmentRepository(
                new DatabaseContextBuilder().WithAppointment(
                    startDateTime,
                    endDateTime,
                    false,
                    false)
                .Build());

            // Act
            Appointment? appointment = await this._appointmentRepository.GetAsync(startDateTime, It.IsAny<CancellationToken>()).ConfigureAwait(!false);

            // Assert
            Assert.IsNotNull(appointment);
            Assert.AreEqual(startDateTime, appointment.StartTime);
            Assert.AreEqual(endDateTime, appointment.EndTime);
        }

        [TestMethod]
        public async Task CreateAsync_ThrowsException_WhenAppointmentIsNull()
        {
            // Arrange
            this._appointmentRepository = new AppointmentRepository(new DatabaseContextBuilder().Build());

            // Act

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await _appointmentRepository.CreateAsync(null, It.IsAny<CancellationToken>()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task CreateAsync_CreatesAppointment_WhenAppointIsNotNull()
        {
            // Arrange
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddMinutes(30);

            var appointment = new Appointment
            {
                StartTime = startTime,
                EndTime = endTime,
                IsAttended = false,
                IsDeleted = false
            };

            this._appointmentRepository = new AppointmentRepository(
                new DatabaseContextBuilder().WithAppointment(
                    new DateTime(2024, 03, 31, 15, 30, 00),
                    new DateTime(2024, 03, 31, 16, 00, 00),
                    false,
                    false)
                .Build());

            // Act
            var result = await this._appointmentRepository.CreateAsync(appointment, It.IsAny<CancellationToken>() ).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(appointment.Id, result.Id);
            Assert.AreEqual(appointment.StartTime, result.StartTime);
            Assert.AreEqual(appointment.EndTime, result.EndTime);
            Assert.AreEqual(appointment.IsAttended, result.IsAttended);
            Assert.AreEqual(appointment.IsDeleted, result.IsDeleted);
        }

        [TestMethod]
        public async Task UpdateAsync_ThrowsException_WhenAppointmentIsNull()
        {
            // Arrange
            this._appointmentRepository = new AppointmentRepository(new DatabaseContextBuilder().Build());

            // Act

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await _appointmentRepository.UpdateAsync(null, It.IsAny<CancellationToken>()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpdateAsync_UpdatesAppointment_WhenAppointIsNotNull()
        {
            // Arrange
            DateTime startTime = new DateTime(2024, 02, 28, 15, 00, 00);
            DateTime endTime = startTime.AddMinutes(30);

            DateTime startTimeToUpdate = startTime.AddHours(1);
            DateTime endTimeToUpdate = endTime.AddHours(1);

            this._appointmentRepository = new AppointmentRepository(
                new DatabaseContextBuilder().WithAppointment(
                    startTime,
                    endTime,
                    false,
                    false)
                .Build());

            var appointmentToUpdate = await this._appointmentRepository.GetAsync(startTime, It.IsAny<CancellationToken>());

            appointmentToUpdate.StartTime = startTimeToUpdate;
            appointmentToUpdate.EndTime = endTimeToUpdate;
            appointmentToUpdate.IsAttended = true;

            // Act
            var result = await this._appointmentRepository.UpdateAsync(appointmentToUpdate, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(startTimeToUpdate, result.StartTime);
            Assert.AreEqual(endTimeToUpdate, result.EndTime);
            Assert.AreEqual(true, result.IsAttended);
        }

        [TestMethod]
        public async Task DeleteAsync_ThrowsException_WhenAppointmentIsNull()
        {
            // Arrange
            this._appointmentRepository = new AppointmentRepository(new DatabaseContextBuilder().Build());

            // Act

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await _appointmentRepository.DeleteAsync(null, It.IsAny<CancellationToken>()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteAsync_DeletesAppointment_WhenAppointIsNotNull()
        {
            // Arrange
            DateTime startTime = new DateTime(2024, 01, 26, 11, 30, 00);
            DateTime endTime = startTime.AddMinutes(30);

            this._appointmentRepository = new AppointmentRepository(
                new DatabaseContextBuilder().WithAppointment(
                    startTime,
                    endTime,
                    false,
                    false)
                .Build());

            Appointment? appointmentToDelete = await this._appointmentRepository.GetAsync(startTime, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Act
            var result = await this._appointmentRepository.DeleteAsync(appointmentToDelete, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);

            var appointments = await this._appointmentRepository.ListAsync(startTime.Date, It.IsAny<CancellationToken>()).ConfigureAwait(false);
            Assert.AreEqual(0, appointments.Count);
        }
    }
}