using Calender.Persistent.Entities;

namespace Calender.Persistent.Repositories
{
    /// <summary>
    /// Service to add, delete and list appointments in the database.
    /// </summary>
    public interface IAppointmentRepository
    {
        /// <summary>
        /// Lists all <see cref="Appointment" />s for the given date, or for today's date.
        /// </summary>
        /// <param name="date"><see cref="DateTime" /> to list all the appointments by date.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
        /// <returns><see cref="Task{List{Appointment}}" />.</returns>
        Task<List<Appointment>> ListAsync(DateTime date, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an appointment for the given date and the start time. If the date is not provided, it gets an appointment for today's date and given start time.
        /// </summary>
        /// <param name="startTime"><see cref="DateTime" /> to get the appointment by start time.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
        /// <returns><see cref="Appointment" />.</returns>
        Task<Appointment?> GetAsync(DateTime startTime, CancellationToken cancellationToken);

        /// <summary>
        /// Creates an appointment.
        /// </summary>
        /// <param name="appointment"><see cref="Appointment" />.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
        /// <returns>Created <see cref="Appointment" />.</returns>
        Task<Appointment> CreateAsync(Appointment appointment, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an appointment.
        /// </summary>
        /// <param name="appointment"><see cref="Appointment" />.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
        /// <returns>Updated <see cref="Appointment" />.</returns>
        Task<Appointment> UpdateAsync(Appointment appointment, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes an appointment.
        /// </summary>
        /// <param name="appointment"><see cref="Appointment" />.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
        /// <returns><see langword="true or false" /></returns>
        Task<bool> DeleteAsync(Appointment appointment, CancellationToken cancellationToken);
    }
}