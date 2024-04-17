using Calender.Business.Models;

namespace Calender.Business.Services
{
    /// <summary>
    /// Services to add, delete, find and keep appointments.
    /// </summary>
    public interface IAppointmentService
    {
        /// <summary>
        /// Finds available timeslots on a given date.
        /// </summary>
        /// <param name="date"><see cref="DateTime "/> to find available timeslots.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
        /// <returns><see cref="Task{TResult} "/> with a <see cref="List{T}" /> of <see cref="AppointmentModel "/>s.</returns>
        Task<List<AppointmentModel>> FindAvailableTimeslots(DateTime date, CancellationToken cancellationToken);

        /// <summary>
        /// Adds an appointment.
        /// </summary>
        /// <param name="appointment"><see cref="AppointmentModel" />.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
        /// <returns><see cref="AppointmentModel" />.</returns>
        Task<AppointmentModel?> AddAppointment(AppointmentModel appointment, CancellationToken cancellationToken);

        /// <summary>
        /// Keeps an appointment for the given date and time.
        /// </summary>
        /// <param name="startTime"><see cref="DateTime" /> to keep the appointment time.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
        /// <returns><see cref="AppointmentModel" />.</returns>
        Task<AppointmentModel> KeepAppointment(DateTime startTime, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes an appointment.
        /// </summary>
        /// <param name="startTime"><see cref="DateTime" /> to delete the appointment starting at the given start time.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
        /// <returns><see langword="true or false" /></returns>
        Task<bool> DeleteAppointment(DateTime startTime, CancellationToken cancellationToken);
    }
}