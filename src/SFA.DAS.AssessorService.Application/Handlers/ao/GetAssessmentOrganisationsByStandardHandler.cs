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
    public class GetAssessmentOrganisationsByStandardHandler : IRequestHandler<GetAssessmentOrganisationsbyStandardRequest, List<EpaOrganisation>>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetAssessmentOrganisationsByStandardHandler> _logger;

        public GetAssessmentOrganisationsByStandardHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetAssessmentOrganisationsByStandardHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<List<EpaOrganisation>> Handle(GetAssessmentOrganisationsbyStandardRequest request, CancellationToken cancellationToken)
        {
            var standardId = request.StandardId;
            _logger.LogInformation($@"Handling AssessmentOrganisations Request for StandardId [{standardId}]");
            var organisations = await _registerQueryRepository.GetAssessmentOrganisationsByStandardId(request.StandardId);
            return organisations.ToList();
        }
    }
}

