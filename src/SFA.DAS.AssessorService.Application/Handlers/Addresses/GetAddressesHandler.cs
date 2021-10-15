using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.AssessorService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Handlers.Addresses
{
    public class GetAddressesHandler
    {
        private readonly ILogger<GetAddressesHandler> _logger;
        private readonly IOuterApiService _outerApiService;

        public GetAddressesHandler(ILogger<GetAddressesHandler> logger, IOuterApiService outerApiService)
        {
            _logger = logger;
            _outerApiService = outerApiService;
        }

        public async Task<List<AddressResponse>> Handle(GetAddressesRequest getAddressesRequest, CancellationToken cancellationToken)
        {
            var response = await _outerApiService.GetAddresses(getAddressesRequest.Query);
            return Mapper.Map<List<AddressResponse>>(response.Addresses);
        }
    }
}
