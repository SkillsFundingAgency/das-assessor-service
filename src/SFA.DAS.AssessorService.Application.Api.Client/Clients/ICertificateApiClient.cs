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
        Task<Certificate> StartPrivate(StartCertificatePrivateRequest request);
        Task<Certificate> GetCertificate(Guid certificateId);
        Task<Certificate> GetCertificate(int ukprn, long uln, string lastName, string endPointAssessorOrganisationId);
        Task<GetPrivateCertificateAlreadySubmittedResponse> GetAlreadySubmittedPrivateCertificate(Guid certificateId);
        Task<List<CertificateSummaryResponse>> GetDraftCertificatesInApprovalState();
        Task<Certificate> UpdateCertificate(UpdateCertificateRequest updateGradeRequest);
        Task<PaginatedList<CertificateSummaryResponse>> GetCertificateHistory(int pageIndex, string userName);
        Task<CertificateAddress> GetContactPreviousAddress(string userName, bool isPrivatelyFunded);
        Task<List<CertificateAddress>> GetPreviousAddressess(string userName);
        Task<List<Option>> GetOptions(int stdCode);
    }
}