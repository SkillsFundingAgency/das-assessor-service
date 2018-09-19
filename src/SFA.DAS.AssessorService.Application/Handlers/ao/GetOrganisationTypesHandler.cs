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
    public class GetOrganisationTypesHandler : IRequestHandler<GetOrganisationTypesRequest, List<OrganisationType>>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetOrganisationTypesHandler> _logger;

        public GetOrganisationTypesHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetOrganisationTypesHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<List<OrganisationType>> Handle(GetOrganisationTypesRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetOrganisationsType Request");
            var result = await _registerQueryRepository.GetOrganisationTypes();
            return result.ToList();
        }
    }
}
