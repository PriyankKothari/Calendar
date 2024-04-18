using Calender.Business.Models;
using Calender.Persistent.Entities;
using Calender.Persistent.Repositories;

namespace Calender.Business.Services
{
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public sealed class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        
        private readonly TimeSpan _workHoursStartTimeSpan;
        private readonly TimeSpan _workHoursEndTimeSpan;
        private readonly TimeSpan _reservedHoursStartTimeSpan;
        private readonly TimeSpan _reservedHoursEndTimeSpan;

        /// <summary>
        /// Creates a new instance of the <see cref="IAppointmentService" />.
        /// </summary>
        /// <param name="appointmentRepository"><see cref="IAppointmentRepository" />.</param>
        /// <param name="workHoursStartTimeSpan"><see cref="TimeSpan" /> indicating the start of working hours.</param>
        /// <param name="workHoursEndTimeSpan"><see cref="TimeSpan "/> indicating the end of working hours.</param>
        /// <param name="reservedHoursStartTimeSpan"><see cref="TimeSpan" /> indicating the start of reserved hours.</param>
        /// <param name="reservedHoursEndTimeSpan"><see cref="TimeSpan" /> indicating the end of reserved hours.</param>
        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            TimeSpan workHoursStartTimeSpan,
            TimeSpan workHoursEndTimeSpan,
            TimeSpan reservedHoursStartTimeSpan,
            TimeSpan reservedHoursEndTimeSpan)
        {
            this._appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));

            this._workHoursStartTimeSpan = workHoursStartTimeSpan;
            this._workHoursEndTimeSpan = workHoursEndTimeSpan;
            this._reservedHoursStartTimeSpan = reservedHoursStartTimeSpan;
            this._reservedHoursEndTimeSpan = reservedHoursEndTimeSpan;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<List<AppointmentModel>> FindAvailableTimeslots(DateTime date, CancellationToken cancellationToken)
        {
            if (date == default)
                throw new ArgumentNullException(nameof(date));

            List<AppointmentModel> availableAppointments = new();

            try
            {
                DateTime startTime = date.Date + _workHoursStartTimeSpan;
                DateTime endTime = date.Date + _workHoursEndTimeSpan;

                List<Appointment> appointments = await this._appointmentRepository.ListAsync(date, cancellationToken).ConfigureAwait(false);

                while (startTime < endTime)
                {
                    if (!appointments.Any(app => app.StartTime == startTime))
                    {
                        availableAppointments.Add(new AppointmentModel { StartTime = startTime, EndTime = startTime.AddMinutes(30) });
                    }
                    startTime = startTime.AddMinutes(30);
                }
            }
            catch
            {
                throw;
            }

            return availableAppointments;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<AppointmentModel?> AddAppointment(AppointmentModel appointment, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(appointment, nameof(appointment));

            AppointmentModel? appointmentModel = null;

            try
            {
                // check if appointment for the given date and time already exists. Throw exception if it does.
                Appointment? existingAppointment = await this._appointmentRepository.GetAsync(appointment.StartTime, cancellationToken).ConfigureAwait(false);
                
                if (existingAppointment is not null)
                    throw new ArgumentException($"Appointment on the given date & time {appointment.StartTime} already exists!");

                DateTime startTime = appointment.StartTime;
                DateTime endTime = appointment.StartTime.AddMinutes(30);

                if (IsAppointmentTimeValid(startTime))
                {
                    // create new appointment if there isn't an appointment already for the given date and time.
                    // TODO: Use automapper
                    Appointment? entity = await this._appointmentRepository.CreateAsync(
                        new Appointment
                        {
                            StartTime = startTime,
                            EndTime = endTime,
                        }, cancellationToken);

                    // TODO: Use automapper
                    appointmentModel = new AppointmentModel
                    {
                        StartTime = entity.StartTime,
                        EndTime = entity.EndTime,
                        IsAttended = entity.IsAttended
                    };
                }
            }
            catch
            {
                throw;
            }

            return appointmentModel;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<AppointmentModel> KeepAppointment(DateTime startTime, CancellationToken cancellationToken)
        {
            if (startTime == default)
                throw new ArgumentNullException(nameof(startTime));

            AppointmentModel? appointmentModel;
            try
            {
                // check if appointment for the given date and time already exists. Throw exception if it does.
                Appointment? existingAppointment = await this._appointmentRepository.GetAsync(startTime, cancellationToken).ConfigureAwait(false);
                
                if (existingAppointment is null)
                    throw new ArgumentException($"Appointment for the given date & time {startTime} doesn't exist!");

                // Update existing appointment's IsAttended for the given date and time.
                existingAppointment.IsAttended = true;
                Appointment? entity = await this._appointmentRepository.UpdateAsync(existingAppointment, cancellationToken);

                // TODO: Use automapper
                appointmentModel = new AppointmentModel
                {
                    StartTime = entity.StartTime,
                    EndTime = entity.EndTime,
                    IsAttended = entity.IsAttended
                };
            }
            catch
            {
                throw;
            }

            return appointmentModel;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<bool> DeleteAppointment(DateTime startTime, CancellationToken cancellationToken)
        {
            if (startTime == default)
                throw new ArgumentNullException(nameof(startTime));

            bool isDeleted;
            try
            {
                // check if appointment for the given date and time already exists. Throw exception if it does.
                Appointment? existingAppointment = await this._appointmentRepository.GetAsync(startTime, cancellationToken).ConfigureAwait(false);
                
                if (existingAppointment is null)
                    throw new ArgumentException($"Appointment for the given date & time {startTime} doesn't exist!");

                // Delete existing appointment for the given date and time.
                isDeleted = await this._appointmentRepository.DeleteAsync(existingAppointment, cancellationToken);
            }
            catch
            {
                throw;
            }

            return isDeleted;
        }

        private bool IsAppointmentTimeValid(DateTime startTime)
        {
            // Check if the start time falls between 09:00 and 17:00
            var isWithinWorkingHours =
                startTime.Hour >= _workHoursStartTimeSpan.Hours &&
                startTime.Hour < _workHoursEndTimeSpan.Hours;

            // Check if the start time is between 4 PM and 5 PM on each second day of the third week of any month
            var isReservedTime =
                startTime.Hour == _reservedHoursStartTimeSpan.Hours &&
                startTime.Hour < _reservedHoursEndTimeSpan.Hours &&
                IsSecondDayOfThirdWeek(startTime) &&
                isWithinWorkingHours;
            
            return isWithinWorkingHours && !isReservedTime;
        }

        private static bool IsSecondDayOfThirdWeek(DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var firstDayOfWeek = firstDayOfMonth.DayOfWeek;
            var firstMonday = firstDayOfMonth.AddDays((firstDayOfWeek == DayOfWeek.Monday ? 1 : 8) - (int)firstDayOfWeek);
            var thirdWeekSecondDay = firstMonday.AddDays(15); // Second day of the third week
            return date.Day == thirdWeekSecondDay.Day;
        }
    }
}
