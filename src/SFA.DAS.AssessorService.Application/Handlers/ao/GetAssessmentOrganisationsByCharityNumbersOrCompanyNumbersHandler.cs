﻿using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetAssessmentOrganisationsByCharityNumbersOrCompanyNumbersHandler : IRequestHandler<GetAssessmentOrganisationsByCharityNumbersOrCompanyNumbersRequest, List<AssessmentOrganisationSummary>>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;

        public GetAssessmentOrganisationsByCharityNumbersOrCompanyNumbersHandler(IRegisterQueryRepository registerQueryRepository)
        {
            _registerQueryRepository = registerQueryRepository;
        }

        public async Task<List<AssessmentOrganisationSummary>> Handle(GetAssessmentOrganisationsByCharityNumbersOrCompanyNumbersRequest request, CancellationToken cancellationToken)
        {
            var results = await _registerQueryRepository.GetAssessmentOrganisationsCharityNumbersOrCompanyNumbers(request.Numbers.ToArray());
            return results.Distinct(new AssessmentOrganisationSummary.EqualityComparer()).Where(x => x.Status != "Applying").ToList();
        }
    }
}
