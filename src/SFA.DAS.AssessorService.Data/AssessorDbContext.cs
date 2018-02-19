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

        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<CertificateLog> CertificateLogs { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
    }
}