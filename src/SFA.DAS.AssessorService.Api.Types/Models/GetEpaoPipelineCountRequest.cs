using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetEpaoPipelineCountRequest: IRequest<EpaoPipelineCountResponse>
    {
        public GetEpaoPipelineCountRequest(string endPointAssessorOrganisationId)
        {
            EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
        }
        public string EndPointAssessorOrganisationId { get; }
    }
}
