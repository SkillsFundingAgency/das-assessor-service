using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models
{ 
    public class GetAssessmentOrganisationsbyStandardRequest : IRequest<List<EpaOrganisation>>
    {
        public int StandardId { get; set; }
    }
}
