using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using MediatR;

    public class GetPreviousAddressesRequest : IRequest<List<CertificateAddressResponse>>
    {
        public string Username { get; set; }
    }
}
