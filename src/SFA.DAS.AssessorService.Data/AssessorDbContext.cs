namespace SFA.DAS.AssessorService.Data
{
    using Microsoft.EntityFrameworkCore;
    using SFA.DAS.AssessorService.Domain.Entities;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class AssessorDbContext : DbContext
    {
        public AssessorDbContext()
        {

        }

        public AssessorDbContext(DbContextOptions<AssessorDbContext> options, bool migrate)
          : base(options)
        {
            if (migrate)
            {
                Database.Migrate();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Organisation>()
                .HasMany(c => c.Contacts)
                .WithOne(e => e.Organisation);
        }

        public override int SaveChanges()
        {
            DateTime saveTime = DateTime.Now;
            foreach (var entry in this.ChangeTracker.Entries()
                .Where(e => e.State == (EntityState)EntityState.Added))
            {
                if ((entry.Property("CreatedAt").CurrentValue == null) || (DateTime)entry.Property("CreatedAt").CurrentValue == DateTime.MinValue)
                    entry.Property("CreatedAt").CurrentValue = saveTime;
            }

            foreach (var entry in this.ChangeTracker.Entries()
              .Where(e => e.State == (EntityState)EntityState.Modified))
            {
                entry.Property("UpdatedAt").CurrentValue = saveTime;
            }
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var addedEntities = ChangeTracker.Entries().Where(E => E.State == EntityState.Added).ToList();
            addedEntities.ForEach(E =>
            {
                E.Property("CreatedAt").CurrentValue = DateTime.Now;
            });

            var editedEntities = ChangeTracker.Entries().Where(E => E.State == EntityState.Modified).ToList();
            editedEntities.ForEach(E =>
            {
                E.Property("UpdatedAt").CurrentValue = DateTime.Now;
            });

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public virtual DbSet<Certificate> Certificates { get; set; }
        public virtual DbSet<CertificateLog> CertificateLogs { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Organisation> Organisations { get; set; }

        public virtual void MarkAsModified<T>(T item) where T : class
        {
            this.Entry<T>(item).State = EntityState.Modified;
        }
    }
}