using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.ConfigurationBuilders
{
    public class CertificateConfigurationBuilder : BaseConfigurator<Certificate>
    {
        private readonly ModelBuilder _modelBuilder;

        public CertificateConfigurationBuilder(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public void Build()
        {
            _modelBuilder.Entity<Certificate>()
                .Property(q => q.EndPointAssessorCertificateId)
                .IsRequired();

            _modelBuilder.Entity<Certificate>()
                .Property(q => q.CertificateData)
                .IsRequired();

            _modelBuilder.Entity<Certificate>()
                .Property(q => q.Status)
                .HasMaxLength(20)
                .IsRequired();

            _modelBuilder.Entity<Certificate>()
                .Property(q => q.CreatedBy)
                .HasMaxLength(30)
                .IsRequired();

            _modelBuilder.Entity<Certificate>()
                .Property(q => q.UpdatedBy)
                .HasMaxLength(30)
                .IsRequired(false);

            _modelBuilder.Entity<Certificate>()
                .Property(q => q.DeletedBy)
                .HasMaxLength(30)
                .IsRequired(false);
        }
    }
}
