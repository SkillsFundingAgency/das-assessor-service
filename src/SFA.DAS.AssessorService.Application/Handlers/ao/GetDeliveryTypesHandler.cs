using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetDeliveryAreasHandler : IRequestHandler<GetDeliveryAreasRequest, List<DeliveryArea>>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetDeliveryAreasHandler> _logger;

        public GetDeliveryAreasHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetDeliveryAreasHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<List<DeliveryArea>> Handle(GetDeliveryAreasRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetDeliveryAreas Request");
            var result = await _registerQueryRepository.GetDeliveryAreas();
            return result.ToList();
        }
    }
}
