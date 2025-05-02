using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.Interfaces
{
    public interface IAssessorDbContext
    {
        DbSet<ApplyEF> Applications { get; set; }
        DbSet<BatchLog> BatchLogs { get; set; }
        DbSet<CertificateBatchLog> CertificateBatchLogs { get; set; }
        DbSet<CertificateLog> CertificateLogs { get; set; }
        DbSet<ContactInvitation> ContactInvitations { get; set; }
        DbSet<Contact> Contacts { get; set; }
        DbSet<ContactsPrivilege> ContactsPrivileges { get; set; }
        DbSet<EMailTemplate> EMailTemplates { get; set; }
        DbSet<FrameworkCertificate> FrameworkCertificates { get; set; }
        DbSet<FrameworkLearner> FrameworkLearners { get; set; }
        DbSet<Ilr> Ilrs { get; set; }
        DbSet<MergeApply> MergeApplications { get; set; }
        DbSet<MergeOrganisation> MergeOrganisations { get; set; }
        DbSet<MergeOrganisationStandardDeliveryArea> MergeOrganisationStandardDeliveryAreas { get; set; }
        DbSet<MergeOrganisationStandard> MergeOrganisationStandards { get; set; }
        DbSet<MergeOrganisationStandardVersion> MergeOrganisationStandardVersions { get; set; }
        DbSet<OfsOrganisation> OfsOrganisation { get; set; }
        DbSet<Organisation> Organisations { get; set; }
        DbSet<OrganisationStandard> OrganisationStandard { get; set; }
        DbSet<OrganisationStandardDeliveryArea> OrganisationStandardDeliveryAreas { get; set; }
        DbSet<OrganisationStandardVersion> OrganisationStandardVersion { get; set; }
        DbSet<Privilege> Privileges { get; set; }
        DbSet<Provider> Providers { get; set; }
        DbSet<SearchLog> SearchLogs { get; set; }
        DbSet<StaffReport> StaffReports { get; set; }
        DbSet<Certificate> StandardCertificates { get; set; }
		DbSet<AssessmentsSummary> AssessmentsSummary { get; set; }
		DbSet<T> Set<T>() where T : class;

        void MarkAsModified<T>(T item) where T : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        DatabaseFacade Database { get; }
    }
}