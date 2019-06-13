using MediatR;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetToBePrintedCertificatesRequest : IRequest<List<CertificateResponse>>
    {

    }
}
