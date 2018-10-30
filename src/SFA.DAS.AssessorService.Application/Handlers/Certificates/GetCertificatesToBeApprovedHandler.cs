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
    public class GetCertificatesToBeApprovedHandler : GetApprovalsBase<GetCertificatesToBeApprovedHandler>,
        IRequestHandler<GetToBeApprovedCertificatesRequest,
        List<CertificateSummaryResponse>>
    {       
        public GetCertificatesToBeApprovedHandler(ICertificateRepository certificateRepository,
            IAssessmentOrgsApiClient assessmentOrgsApiClient,
            ILogger<GetCertificatesToBeApprovedHandler> logger) : base(certificateRepository,
            assessmentOrgsApiClient, logger)
        {           
        }

        public async Task<List<CertificateSummaryResponse>> Handle(GetToBeApprovedCertificatesRequest request,
            CancellationToken cancellationToken)
        {
            var statuses = new List<string>
            {                
                Domain.Consts.CertificateStatus.ToBeApproved,
                Domain.Consts.CertificateStatus.Approved,
                Domain.Consts.CertificateStatus.Rejected,
                Domain.Consts.CertificateStatus.SentForApproval
            };

            var certificates = await CertificateRepository.GetCertificates(statuses);

            // Please Note:- Cannot seem to automap this with custom value/type converters
            // so dealing with it manually for now.
            var approvals = MapCertificates(certificates);
            return approvals;
        }       
    }
}