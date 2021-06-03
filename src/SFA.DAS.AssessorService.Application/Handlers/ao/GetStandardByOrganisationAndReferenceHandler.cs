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
    public class GetStandardByOrganisationAndReferenceHandler : IRequestHandler<GetStandardByOrganisationAndReferenceRequest, OrganisationStandard>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetStandardByOrganisationAndReferenceHandler> _logger;

        public GetStandardByOrganisationAndReferenceHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetStandardByOrganisationAndReferenceHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<OrganisationStandard> Handle(GetStandardByOrganisationAndReferenceRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($@"Handling GetStandardByOrganisationAndReferenceRequest Request for OrganisationId [{request.OrganisationId}] and Standard Reference[{request.StandardReference}]");
            
            var orgStandard = await _registerQueryRepository.GetOrganisationStandardFromOrganisationIdAndStandardRefence(request.OrganisationId, request.StandardReference);
            if (orgStandard != null)
            {
                var versions = await _registerQueryRepository.GetOrganisationStandardVersionsByOrganisationStandardId(orgStandard.Id);
                orgStandard.Versions = versions.ToList();
            }

            return orgStandard;
        }
    } 
}
