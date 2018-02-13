namespace SFA.DAS.AssessorService.Data
{
    using Microsoft.EntityFrameworkCore;
    using SFA.DAS.AssessorService.Data.Entitites;

    public class AssessorDbContext : DbContext
    {
        public AssessorDbContext(DbContextOptions<AssessorDbContext> options)
          : base(options)
        {

        }

        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<CertificateLog> CertificateLogs { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
    }
}