﻿using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface ICertificateApiClient
    {
        Task<Certificate> Start(StartCertificateRequest request);
        Task<Certificate> GetCertificate(Guid certificateId, bool includeLogs = false);
        Task<Certificate> UpdateCertificate(UpdateCertificateRequest updateGradeRequest);
        Task<PaginatedList<CertificateSummaryResponse>> GetCertificateHistory(int pageIndex, string endPointAssessorOrganisationId, string searchTerm, string sortColumn, int sortDescending);
        Task<CertificateAddress> GetContactPreviousAddress(string epaOrgId, long? employerAccountId);
        Task Delete(DeleteCertificateRequest deleteCertificateRequest);
        Task<Certificate> UpdateCertificateRequestReprint(UpdateCertificateRequestReprintCommand command);
        Task UpdateCertificateWithAmendReason(UpdateCertificateWithAmendReasonCommand command);
        Task UpdateCertificateWithReprintReason(UpdateCertificateWithReprintReasonCommand command);
        Task<FrameworkCertificate> ReprintFramework(ReprintFrameworkCertificateRequest request);
    }
}