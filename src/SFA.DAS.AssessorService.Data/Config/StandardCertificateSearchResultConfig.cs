using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.Config
{

    public class StandardCertificateSearchResultConfiguration
        : IEntityTypeConfiguration<StandardCertificateSearchResult>
    {
        public void Configure(
            EntityTypeBuilder<StandardCertificateSearchResult> builder)
        {
            builder.HasNoKey();

            builder.ToView(
                "StandardCertificateSearchView",
                "dbo");
        }
    }
}
