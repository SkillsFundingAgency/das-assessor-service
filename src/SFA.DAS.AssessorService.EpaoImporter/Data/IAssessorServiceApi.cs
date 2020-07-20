using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.DTOs;

namespace SFA.DAS.AssessorService.EpaoImporter.Data
{
    public interface IAssessorServiceApi
    {
        Task UpdatePrivatelyFundedCertificateRequestsToBeApproved();
        Task<IEnumerable<CertificateResponse>> GetCertificatesToBeApproved();
        Task<EmailTemplateSummary> GetEmailTemplate(string templateName);
        Task GatherStandards();
    }
}