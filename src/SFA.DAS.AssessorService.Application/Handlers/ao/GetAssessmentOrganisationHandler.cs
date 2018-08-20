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
    public class GetAssessmentOrganisationHandler : IRequestHandler<GetAssessmentOrganisationRequest, EpaOrganisation>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetAssessmentOrganisationHandler> _logger;

        public GetAssessmentOrganisationHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetAssessmentOrganisationHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<EpaOrganisation> Handle(GetAssessmentOrganisationRequest request, CancellationToken cancellationToken)
        {
            var organisationId = request.OrganisationId;
            _logger.LogInformation($@"Handling AssessmentOrganisation Request for [{organisationId}]");
            var org = await _registerQueryRepository.GetEpaOrganisationByOrganisationId(organisationId);

            return org ?? null;
        }

       
    }
}
