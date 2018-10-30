using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetApprovedDraftCertificatesHandler : GetApprovalsBase<GetApprovedDraftCertificatesHandler>,
        IRequestHandler<GetApprovedDraftCertificatesRequest,
        List<CertificateSummaryResponse>>
    {
        public GetApprovedDraftCertificatesHandler(ICertificateRepository certificateRepository,
            IAssessmentOrgsApiClient assessmentOrgsApiClient,
            ILogger<GetApprovedDraftCertificatesHandler> logger) : base(certificateRepository,
            assessmentOrgsApiClient, logger)
        {
        }

        public async Task<List<CertificateSummaryResponse>> Handle(GetApprovedDraftCertificatesRequest request,
            CancellationToken cancellationToken)
        {
            var certificates = await CertificateRepository.GetApprovedDraftCertificates();

            // Please Note:- Cannot seem to automap this with custom value/type converters
            // so dealing with it manually for now.
            var approvals = MapCertificates(certificates);
            return approvals;
        }
    }
}