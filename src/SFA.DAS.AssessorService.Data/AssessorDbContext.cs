using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class AssessorDbContext : DbContext
    {
        private readonly IDbConnection _connection;

        public AssessorDbContext()
        {
        }

        public AssessorDbContext(IDbConnection connection, DbContextOptions<AssessorDbContext> options)
            : base(options)
        {
            _connection = connection;
        }

        public virtual DbSet<Certificate> Certificates { get; set; }
        public virtual DbSet<CertificateLog> CertificateLogs { get; set; }
        public virtual DbSet<CertificateBatchLog> CertificateBatchLogs { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Organisation> Organisations { get; set; }
        public virtual DbSet<OrganisationStandard> OrganisationStandard { get; set; }
        public virtual DbSet<OrganisationStandardVersion> OrganisationStandardVersion { get; set; }
        public virtual DbSet<OrganisationStandardDeliveryArea> OrganisationStandardDeliveryAreas { get; set; }
        public virtual DbSet<Ilr> Ilrs { get; set; }
        public virtual DbSet<EMailTemplate> EMailTemplates { get; set; }
        public virtual DbSet<BatchLog> BatchLogs { get; set; }
        public virtual DbSet<SearchLog> SearchLogs { get; set; }
        public virtual DbSet<StaffReport> StaffReports { get; set; }
        public virtual DbSet<ContactsPrivilege> ContactsPrivileges { get; set; }
        public virtual DbSet<Privilege> Privileges { get; set; }
        public virtual DbSet<ContactInvitation> ContactInvitations { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }

        public virtual DbSet<MergeOrganisation> MergeOrganisations { get; set; }
        public virtual DbSet<MergeOrganisationStandard> MergeOrganisationStandards { get; set; }
        public virtual DbSet<MergeOrganisationStandardVersion> MergeOrganisationStandardVersions { get; set; }
        public virtual DbSet<MergeOrganisationStandardDeliveryArea> MergeOrganisationStandardDeliveryAreas { get; set; }


        public override int SaveChanges()
        {
            var saveTime = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added && e.Entity is BaseEntity))
                if (entry.Property("CreatedAt").CurrentValue == null ||
                    (DateTime)entry.Property("CreatedAt").CurrentValue == DateTime.MinValue)
                    entry.Property("CreatedAt").CurrentValue = saveTime;

            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified && e.Entity is BaseEntity))
                entry.Property("UpdatedAt").CurrentValue = saveTime;
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var addedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Added && e.Entity is BaseEntity).ToList();
            addedEntities.ForEach(e => { e.Property("CreatedAt").CurrentValue = DateTime.UtcNow; });

            var editedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified && e.Entity is BaseEntity).ToList();
            editedEntities.ForEach(e => { e.Property("UpdatedAt").CurrentValue = DateTime.UtcNow; });

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public virtual void MarkAsModified<T>(T item) where T : class
        {
            Entry(item).State = EntityState.Modified;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connection as DbConnection, options =>
                 options.EnableRetryOnFailure(3).CommandTimeout(300));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.CertificateBatchLog)
                .WithOne(s => s.Certificate)
                .HasForeignKey<CertificateBatchLog>(sc => new { sc.CertificateReference, sc.BatchNumber });
                
            modelBuilder.Entity<CertificateBatchLog>()
                .HasKey(c => new { c.CertificateReference, c.BatchNumber });

            modelBuilder.Entity<CertificateBatchLog>()
                .HasOne(c => c.Certificate)
                .WithOne(c => c.CertificateBatchLog)
                .HasForeignKey<Certificate>(c => new { c.CertificateReference, c.BatchNumber });
            
            modelBuilder.Entity<ContactsPrivilege>()
                .HasKey(sc => new { sc.ContactId, sc.PrivilegeId });

            modelBuilder.Entity<ContactsPrivilege>()
                .HasOne<Contact>(sc => sc.Contact)
                .WithMany(s => s.ContactsPrivileges)
                .HasForeignKey(sc => sc.ContactId);

            modelBuilder.Entity<OrganisationStandard>()
                .ToTable("OrganisationStandard");
            
            modelBuilder.Entity<OrganisationStandard>()
                .HasOne(c => c.Organisation)
                .WithMany(c => c.OrganisationStandards)
                .HasPrincipalKey(c => c.EndPointAssessorOrganisationId)
                .HasForeignKey(c => c.EndPointAssessorOrganisationId);
            
            modelBuilder.Entity<OrganisationStandardVersion>()
                .ToTable("OrganisationStandardVersion")
                .HasKey(c => new { c.OrganisationStandardId, c.StandardUId, c.Version });

            modelBuilder.Entity<OrganisationStandardVersion>()
                .HasOne(c => c.OrganisationStandard)
                .WithMany(c => c.OrganisationStandardVersions)
                .HasPrincipalKey(c => c.Id)
                .HasForeignKey(c => c.OrganisationStandardId);

            modelBuilder.Entity<OrganisationStandardDeliveryArea>()
                .ToTable("OrganisationStandardDeliveryArea");
            
            modelBuilder.Entity<OrganisationStandardDeliveryArea>()
                .HasOne(c => c.OrganisationStandard)
                .WithMany(c => c.OrganisationStandardDeliveryAreas)
                .HasPrincipalKey(c => c.Id)
                .HasForeignKey(c => c.OrganisationStandardId);

            modelBuilder.Entity<OrganisationStandardDeliveryArea>()
                .HasOne(c => c.DeliveryArea)
                .WithOne(c => c.OrganisationStandardDeliveryArea)
                .HasForeignKey<DeliveryArea>(c => c.Id);

            modelBuilder.Entity<DeliveryArea>()
                .ToTable("DeliveryArea");

            modelBuilder.Entity<Provider>()
                .ToTable("Providers")
                .HasKey(c => new { c.Ukprn });



            modelBuilder.Entity<MergeOrganisation>()
                .ToTable("MergeOrganisations")
                .HasMany(e => e.MergeOrganisationStandards)
                .WithOne(e => e.MergeOrganisation);


            modelBuilder.Entity<MergeOrganisationStandard>()
                .ToTable("MergeOrganisationStandard");




            modelBuilder.Entity<MergeOrganisationStandardVersion>()
                .ToTable("MergeOrganisationStandardVersion")
                //.HasKey(e => new { e.OrganisationStandardId, e.StandardUid, e.Version })
                ;


            modelBuilder.Entity<MergeOrganisationStandardDeliveryArea>()
                .ToTable("MergeOrganisationStandardDeliveryArea")
                ;

            SetUpJsonToEntityTypeHandlers(modelBuilder);
        }

        private void SetUpJsonToEntityTypeHandlers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SearchLog>()
                .Property(e => e.SearchData)
                .HasConversion(
                    c => JsonConvert.SerializeObject(c),
                    c => JsonConvert.DeserializeObject<SearchData>(string.IsNullOrWhiteSpace(c) ? "{}" : c));

            modelBuilder.Entity<Privilege>()
                .Property(e => e.PrivilegeData)
                .HasConversion(
                    c => JsonConvert.SerializeObject(c),
                    c => JsonConvert.DeserializeObject<PrivilegeData>(string.IsNullOrWhiteSpace(c) ? "{}" : c));
            
            modelBuilder.Entity<Organisation>()
               .Property(e => e.OrganisationData)
               .HasConversion(
                   c => JsonConvert.SerializeObject(c),
                   c => JsonConvert.DeserializeObject<OrganisationData>(string.IsNullOrWhiteSpace(c) ? "{}" : c));

            modelBuilder.Entity<BatchLog>()
                 .Property(e => e.BatchData)
                 .HasConversion(
                     c => JsonConvert.SerializeObject(c),
                     c => JsonConvert.DeserializeObject<BatchData>(string.IsNullOrWhiteSpace(c) ? "{}" : c));
        }
    }
}