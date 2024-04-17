using Calender.Persistent.DatabaseContexts;
using Calender.Persistent.Entities;
using Microsoft.EntityFrameworkCore;

namespace Calender.Persistent.Repositories
{
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public sealed class AppointmentRepository : IAppointmentRepository
    {
        private readonly DatabaseContext _databaseContext;

        /// <summary>
        /// Initiate a new instance of <see cref="IAppointmentRepository" />.
        /// </summary>
        /// <param name="databaseContext"></param>
        public AppointmentRepository(DatabaseContext databaseContext)
        {
            this._databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<List<Appointment>> ListAsync(DateTime date, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(date, nameof(date));

            try
            {
                return await
                    this._databaseContext.Set<Appointment>()
                    .AsNoTracking()
                    .Where(app => app.StartTime.Date == date)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                // log exception and continue
                throw;
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<Appointment?> GetAsync(DateTime startTime, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(startTime, nameof(startTime));

            try
            {
                return await
                    this._databaseContext.Set<Appointment>()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(app => app.StartTime.Date == startTime.Date && app.StartTime.Hour == startTime.Hour && app.StartTime.Minute == startTime.Minute, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                // log exception and continue
                throw;
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<Appointment> CreateAsync(Appointment appointment, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(appointment, nameof(appointment));

            try
            {
                this._databaseContext.Add(appointment);
                await this._databaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return appointment;
            }
            catch (DbUpdateConcurrencyException exception)
            {
                // log exception and continue
                throw new DbUpdateConcurrencyException(exception.Message);
            }
            catch (Exception exception)
            {
                // log exception and continue
                throw new Exception(exception.InnerException?.Message);
            }
        }

        public async Task<Appointment> UpdateAsync(Appointment appointment, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(appointment, nameof(appointment));

            try
            {
                var localAppointment =
                    this._databaseContext.Set<Appointment>()
                    .Local
                    .FirstOrDefault(app => app.StartTime.Hour == appointment.StartTime.Hour && app.StartTime.Minute == appointment.StartTime.Minute);

                if (localAppointment != null)
                    this._databaseContext.Entry((object)localAppointment).State = EntityState.Detached;

                this._databaseContext.Update(appointment);
                await this._databaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return appointment;
            }
            catch (DbUpdateConcurrencyException exception)
            {
                // log exception and continue
                throw new DbUpdateConcurrencyException(exception.Message);
            }
            catch (Exception exception)
            {
                // log exception and continue
                throw new Exception(exception.InnerException?.Message);
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<bool> DeleteAsync(Appointment appointment, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(appointment, nameof(appointment));

            try
            {
                this._databaseContext.Remove(appointment);
                await this._databaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (Exception exception)
            {
                // log exception and continue
                return false;
            }
        }
    }
}