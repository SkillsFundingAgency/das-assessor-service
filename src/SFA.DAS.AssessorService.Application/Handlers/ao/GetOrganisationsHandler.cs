using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetOrganisationsHandler : IRequestHandler<GetOrganisationsRequest, List<OrganisationTypeResponse>>
    {
        public async Task<List<OrganisationTypeResponse>> Handle(GetOrganisationsRequest request, CancellationToken cancellationToken)
        {
            var organisationTypes = new List<OrganisationTypeResponse>
            {
                new OrganisationTypeResponse {Id = Guid.NewGuid(), OrganisationType = "abc"},
                new OrganisationTypeResponse {Id = Guid.NewGuid(), OrganisationType = "def"}
            };

            return organisationTypes;
        }
    }
}
