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
    /// <summary>
    /// This handler doesn't restrict based on efffective from / to and status of live
    /// As it's called from the Admin Service and the Admin side needs to know about active and withdrawn standards.
    /// </summary>
    public class GetAllStandardsByOrganisationHandler : IRequestHandler<GetAllStandardsByOrganisationRequest, List<OrganisationStandardSummary>>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetAllStandardsByOrganisationHandler> _logger;

        public GetAllStandardsByOrganisationHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetAllStandardsByOrganisationHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<List<OrganisationStandardSummary>> Handle(GetAllStandardsByOrganisationRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($@"Handling OrganisationStandards Request for OrganisationId [{request.OrganisationId}]");
            var orgStandards = await _registerQueryRepository.GetAllOrganisationStandardByOrganisationId(request.OrganisationId);
            return orgStandards.ToList();
        }
    } 
}
