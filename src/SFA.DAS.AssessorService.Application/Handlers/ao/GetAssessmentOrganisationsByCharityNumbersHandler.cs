using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetAssessmentOrganisationsByCharityNumbersHandler : IRequestHandler<GetAssessmentOrganisationsByCharityNumbersRequest, List<AssessmentOrganisationSummary>>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;

        public GetAssessmentOrganisationsByCharityNumbersHandler(IRegisterQueryRepository registerQueryRepository)
        {
            _registerQueryRepository = registerQueryRepository;
        }

        public async Task<List<AssessmentOrganisationSummary>> Handle(GetAssessmentOrganisationsByCharityNumbersRequest request, CancellationToken cancellationToken)
        {
            var results = await _registerQueryRepository.GetAssessmentOrganisationsByCompanyNumbers(request.CharityNumbers.ToArray());
            return results.Distinct(new AssessmentOrganisationSummary.EqualityComparer()).Where(x => x.Status != "Applying").ToList();
        }
    }
}

