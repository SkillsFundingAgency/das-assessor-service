using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.ConfigurationBuilders
{
    public class CertificateLogConfigurationBuilder : BaseConfigurator<CertificateLog>
    {
        private readonly ModelBuilder _modelBuilder;

        public CertificateLogConfigurationBuilder(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public void Build()
        {
            _modelBuilder.Entity<CertificateLog>()
                .Property(q => q.Action)
                .HasMaxLength(400)
                .IsRequired();

            _modelBuilder.Entity<CertificateLog>()
               .Property(q => q.Status)
               .HasMaxLength(12)
               .IsRequired();

            _modelBuilder.Entity<CertificateLog>()
                .HasAlternateKey(q => q.EventTime);
        }
    }
}
