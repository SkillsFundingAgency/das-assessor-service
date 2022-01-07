using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetAllStandardsByOrganisationRequest: IRequest<List<OrganisationStandardSummary>>
    {
        public string OrganisationId;
    }
}
