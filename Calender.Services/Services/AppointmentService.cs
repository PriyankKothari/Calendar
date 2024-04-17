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

        /// <summary>
        /// Creates a new instance of the <see cref="IAppointmentService" />.
        /// </summary>
        /// <param name="appointmentRepository"><see cref="IAppointmentRepository" />.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            this._appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<List<AppointmentModel>> FindAvailableTimeslots(DateTime date, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(date, nameof(date));

            List<AppointmentModel> availableAppointments = new List<AppointmentModel>();

            try
            {
                DateTime startTime = new DateTime(date.Year, date.Month, date.Day, 09, 00, 00);
                DateTime endTime = new DateTime(date.Year, date.Month, date.Day, 17, 00, 00);

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
                if (existingAppointment != null)
                    throw new ArgumentException($"Appointment on the given date & time {appointment.StartTime} already exists!");

                DateTime startTime = appointment.StartTime;
                DateTime endTime = appointment.StartTime.AddMinutes(30);

                if (IsAppointmentTimeValid(startTime, endTime))
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
            ArgumentNullException.ThrowIfNull(startTime, nameof(startTime));

            AppointmentModel? appointmentModel;
            try
            {
                // check if appointment for the given date and time already exists. Throw exception if it does.
                Appointment? existingAppointment = await this._appointmentRepository.GetAsync(startTime, cancellationToken).ConfigureAwait(false);
                if (existingAppointment == null)
                    throw new ArgumentException($"Appointment for the given date & time {startTime} doesn't exist!");

                // Update existing appointment for the given date and time.
                // TODO: Use automapper                
                Appointment? entity = await this._appointmentRepository.UpdateAsync(
                    new Appointment
                    {
                        Id = existingAppointment.Id,
                        StartTime = existingAppointment.StartTime,
                        EndTime = existingAppointment.EndTime,
                        IsAttended = true
                    }, cancellationToken);

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
            ArgumentNullException.ThrowIfNull(startTime, nameof(startTime));

            bool isDeleted;
            try
            {
                // check if appointment for the given date and time already exists. Throw exception if it does.
                Appointment? existingAppointment = await this._appointmentRepository.GetAsync(startTime, cancellationToken).ConfigureAwait(false);
                if (existingAppointment == null)
                    throw new ArgumentException($"Appointment for the given date & time {startTime} doesn't exist!");

                // Delete existing appointment for the given date and time.
                // TODO: Use automapper
                isDeleted = await this._appointmentRepository.DeleteAsync(
                    new Appointment
                    {
                        Id = existingAppointment.Id,
                        StartTime = existingAppointment.StartTime,
                        EndTime = existingAppointment.EndTime
                    }, cancellationToken);
            }
            catch
            {
                throw;
            }

            return isDeleted;
        }

        private static bool IsAppointmentTimeValid(DateTime startTime, DateTime endTime)
        {
            // Check if the start time falls between 09:00 and 17:00
            var startOfDay = startTime.Date.AddHours(9);
            var endOfDay = startTime.Date.AddHours(17);
            var isWithinWorkingHours = startTime >= startOfDay && startTime < endOfDay;

            // Check if the start time is between 4 PM and 5 PM on each second day of the third week of any month
            var isReservedTime = startTime.Hour == 16 && startTime.Hour < 17 && IsSecondDayOfThirdWeek(startTime) && isWithinWorkingHours;
            
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
