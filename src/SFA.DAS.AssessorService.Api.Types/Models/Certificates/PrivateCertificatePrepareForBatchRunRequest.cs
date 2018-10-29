using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class PrivateCertificatePrepareForBatchRunRequest : IRequest
    {
        public string UserName { get; set; }
    }
}