using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetAddressesRequest : IRequest<List<AddressResponse>>
    {
        public string Query { get; set; }
    }
}
