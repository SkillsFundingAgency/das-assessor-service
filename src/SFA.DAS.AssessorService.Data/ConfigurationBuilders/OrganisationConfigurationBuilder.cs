using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.ConfigurationBuilders
{
    public class OrganisationConfigurationBuilder : BaseConfigurator<Organisation>
    {
        private readonly ModelBuilder _modelBuilder;

        public OrganisationConfigurationBuilder(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public void Build()
        {
            _modelBuilder.Entity<Organisation>()
                .HasMany(c => c.Contacts)
                .WithOne(e => e.Organisation);

            _modelBuilder.Entity<Organisation>()
                .HasAlternateKey(c => c.EndPointAssessorOrganisationId);

            _modelBuilder.Entity<Organisation>()
                .Property(c => c.EndPointAssessorOrganisationId)
                .HasMaxLength(12);

            _modelBuilder.Entity<Organisation>()
                .Property(q => q.Status)
                .IsRequired();

            _modelBuilder.Entity<Organisation>()
                .Property(c => c.EndPointAssessorName)
                .HasMaxLength(100)
                .IsRequired();

            _modelBuilder.Entity<Organisation>()
                .Property(c => c.PrimaryContact)
                .HasMaxLength(30);
        }
    }
}
