using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    // Please note this is empty by desig as there are currently no input paramters  to allow mediator to work.
    public class GetApprovedDraftCertificatesRequest : IRequest<List<CertificateSummaryResponse>>
    {             
    }
}