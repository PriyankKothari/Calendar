using Calender.Persistent.Entities;
using Microsoft.EntityFrameworkCore;

namespace Calender.Persistent.DatabaseContexts
{
    /// <summary>
    /// DatabaseContext to declare <see cref="DbSet{TEntity}"/>s, overriding <see cref="OnModelCreating(ModelBuilder) "> configuration and <see cref="SaveChangesAsync(bool, CancellationToken)" /> method.
    /// </summary>
    public sealed class DatabaseContext : DbContext
    {
        /// <summary>
        /// <see cref="DbSet{Appointment} "/>.
        /// </summary>
        public DbSet<Appointment> Appointments { get; set; }

        /// <summary>
        /// Initiate a new instance of <see cref="DatabaseContext" />.
        /// </summary>
        /// <param name="dbContextOptions"><see cref="DbContextOptions{TContext}" />.</param>
        public DatabaseContext(DbContextOptions<DatabaseContext> dbContextOptions) : base(dbContextOptions)
        {
            base.Database.EnsureCreated();
        }

        /// <summary>
        /// <inheritdoc />.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>().ToTable("Appointment");
            modelBuilder.Entity<Appointment>().HasKey(tag => tag.Id);
            modelBuilder.Entity<Appointment>().Property(tag => tag.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Appointment>().Property(tag => tag.IsAttended).HasDefaultValue(false);
            modelBuilder.Entity<Appointment>().Property(tag => tag.IsDeleted).HasDefaultValue(false);

            modelBuilder.Entity<Appointment>().HasQueryFilter(tag => tag.IsDeleted == false);

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
        {
            // set IsDeleted and DeletedDate when entity is EntityState.Deleted.
            var deletedEntities = ChangeTracker.Entries().Where(entity => entity.State == EntityState.Deleted).ToList();
            deletedEntities.ForEach(entity =>
            {
                entity.State = EntityState.Modified;
                entity.Property("IsDeleted").CurrentValue = true;
            });

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
