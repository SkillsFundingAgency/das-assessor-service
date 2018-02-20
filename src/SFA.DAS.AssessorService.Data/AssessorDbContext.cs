namespace SFA.DAS.AssessorService.Data
{
    using Microsoft.EntityFrameworkCore;    
    using SFA.DAS.AssessorService.Domain.Entities;

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

        public virtual DbSet<Certificate> Certificates { get; set; }
        public virtual DbSet<CertificateLog> CertificateLogs { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Organisation> Organisations { get; set; }

        public virtual void MarkAsModified<T>(T item) where T: class
        {
            this.Entry<T>(item).State = EntityState.Modified;
        }
    }
}