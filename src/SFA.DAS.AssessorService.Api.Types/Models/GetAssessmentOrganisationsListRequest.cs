﻿using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetAssessmentOrganisationsListRequest : IRequest<List<AssessmentOrganisationListSummary>>
    {
        public int? Ukprn { get; }

        public GetAssessmentOrganisationsListRequest()
        {
        }

        public GetAssessmentOrganisationsListRequest(int? ukprn)
        {
            Ukprn = ukprn;
        }
    }
}