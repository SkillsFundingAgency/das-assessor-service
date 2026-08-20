using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.Config
{

    public class FrameworkCertificateSearchResultConfiguration
        : IEntityTypeConfiguration<FrameworkCertificateSearchResult>
    {
        public void Configure(
            EntityTypeBuilder<FrameworkCertificateSearchResult> builder)
        {
            builder.HasNoKey();

            builder.ToView(
                "FrameworkCertificateSearchView",
                "dbo");
        }
    }
}
