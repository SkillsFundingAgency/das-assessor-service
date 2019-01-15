using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetEpaOrganisationPipelineCountRequest: IRequest<EpaOrganisationPipelineCountResponse>
    {
        public GetEpaOrganisationPipelineCountRequest(string endPointAssessorOrganisationId)
        {
            EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
        }
        public string EndPointAssessorOrganisationId { get; }
    }
}
