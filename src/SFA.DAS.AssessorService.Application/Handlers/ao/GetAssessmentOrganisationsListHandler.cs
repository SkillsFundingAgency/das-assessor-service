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
    public class GetAssessmentOrganisationsListHandler : IRequestHandler<GetAparSummaryByUkprnRequest, List<AparSummaryItem>>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetAssessmentOrganisationsListHandler> _logger;

        public GetAssessmentOrganisationsListHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetAssessmentOrganisationsListHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<List<AparSummaryItem>> Handle(GetAparSummaryByUkprnRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAparSummaryByUkprn Request");
            var result = await _registerQueryRepository.GetAparSummaryByUkprn(request.Ukprn);
            return result.ToList();
        }
    }
}
