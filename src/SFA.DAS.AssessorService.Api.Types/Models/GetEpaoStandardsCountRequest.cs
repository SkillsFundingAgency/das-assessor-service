using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetEpaoStandardsCountRequest: IRequest<EpaoStandardsCountResponse>
    {
        public GetEpaoStandardsCountRequest(string endPointAssessorOrganisationId)
        {
            EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
        }
        public string EndPointAssessorOrganisationId { get; }
    }
}
