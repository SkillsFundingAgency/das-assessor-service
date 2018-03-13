using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Data.ConfigurationBuilders;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data
{
    public class AssessorDbContext : DbContext
    {
        public AssessorDbContext()
        {
        }

        public AssessorDbContext(DbContextOptions<AssessorDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Certificate> Certificates { get; set; }
        public virtual DbSet<CertificateLog> CertificateLogs { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Organisation> Organisations { get; set; }
        public virtual DbSet<Ilr> Ilrs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new OrganisationConfigurationBuilder(modelBuilder).Build();
            new ContactConfigurationBuilder(modelBuilder).Build();
            new CertificateConfigurationBuilder(modelBuilder).Build();
            new CertificateLogConfigurationBuilder(modelBuilder).Build();
        }

        public override int SaveChanges()
        {
            var saveTime = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added))
                if (entry.Property("CreatedAt").CurrentValue == null ||
                    (DateTime) entry.Property("CreatedAt").CurrentValue == DateTime.MinValue)
                    entry.Property("CreatedAt").CurrentValue = saveTime;

            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified))
                entry.Property("UpdatedAt").CurrentValue = saveTime;
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var addedEntities = ChangeTracker.Entries().Where(E => E.State == EntityState.Added).ToList();
            addedEntities.ForEach(E => { E.Property("CreatedAt").CurrentValue = DateTime.UtcNow; });

            var editedEntities = ChangeTracker.Entries().Where(E => E.State == EntityState.Modified).ToList();
            editedEntities.ForEach(E => { E.Property("UpdatedAt").CurrentValue = DateTime.UtcNow; });

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public virtual void MarkAsModified<T>(T item) where T : class
        {
            Entry(item).State = EntityState.Modified;
        }
    }
}