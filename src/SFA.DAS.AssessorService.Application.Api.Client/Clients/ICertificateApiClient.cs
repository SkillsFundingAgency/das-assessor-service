using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface ICertificateApiClient
    {
        Task<Certificate> Start(StartCertificateRequest request);
        Task<Certificate> GetCertificate(Guid certificateId, bool includeLogs = false);
        Task<Certificate> UpdateCertificate(UpdateCertificateRequest updateGradeRequest);
        Task<PaginatedList<CertificateSummaryResponse>> GetCertificateHistory(int pageIndex, string endPointAssessorOrganisationId, string searchTerm, string sortColumn, int sortDescending);
        Task<CertificateAddress> GetContactPreviousAddress(string epaOrgId, string employerAccountId);
        Task Delete(DeleteCertificateRequest deleteCertificateRequest);
    }
}