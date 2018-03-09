using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.ConfigurationBuilders
{
    public class BaseConfigurator<T> where T:  BaseEntity
    {
        public BaseConfigurator(ModelBuilder modelBuilder)
        {
            var modelBuilder1 = modelBuilder;

            modelBuilder1.Entity<T>()
                .Property(q => q.CreatedAt)
                .IsRequired();

            modelBuilder1.Entity<T>()
                .Property(q => q.UpdatedAt)
                .IsRequired(false);

            modelBuilder1.Entity<T>()
                .Property(q => q.DeletedAt)
                .IsRequired(false);
        }
    }
}
