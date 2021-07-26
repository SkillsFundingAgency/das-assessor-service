using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetCertificatesReadyToPrintCountRequest : IRequest<int>
    {
        public GetCertificatesReadyToPrintCountRequest()
        {
        }
    }
}