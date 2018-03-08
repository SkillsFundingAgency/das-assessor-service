using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.ConfigurationBuilders
{
    public class ContactConfigurationBuilder : BaseConfigurator<Contact>
    {
        private readonly ModelBuilder _modelBuilder;

        public ContactConfigurationBuilder(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public void Build()
        {
            _modelBuilder.Entity<Contact>()
                .HasAlternateKey(q => q.Username);

            _modelBuilder.Entity<Contact>()
                .Property(q => q.EndPointAssessorOrganisationId)
                .HasMaxLength(12)
                .IsRequired();

            _modelBuilder.Entity<Contact>()
                .Property(q => q.Username)
                .HasMaxLength(30)
                .IsRequired();

            _modelBuilder.Entity<Contact>()
                .Property(q => q.DisplayName)
                .HasMaxLength(120)
                .IsRequired();

            _modelBuilder.Entity<Contact>()
                .Property(q => q.Email)
                .HasMaxLength(120)
                .IsRequired();

            _modelBuilder.Entity<Contact>()
                .Property(q => q.Status)
                .HasMaxLength(10)
                .IsRequired();

        }
    }
}
