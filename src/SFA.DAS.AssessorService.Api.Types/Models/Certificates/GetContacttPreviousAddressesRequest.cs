using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetContactPreviousAddressesRequest : IRequest<CertificateAddress>
    {
        public string EpaOrgId { get; set; }
        public string EmployerAccountId { get; set; }
    }
}
