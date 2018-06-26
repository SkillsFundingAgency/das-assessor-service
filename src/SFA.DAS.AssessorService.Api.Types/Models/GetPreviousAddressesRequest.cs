using System.Collections.Generic;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using MediatR;

    public class GetPreviousAddressesRequest : IRequest<List<CertificateAddress>>
    {
        public string Username { get; set; }
    }
}
