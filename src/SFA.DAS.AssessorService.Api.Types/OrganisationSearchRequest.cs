using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Api.Types
{
    public class OrganisationSearchRequest: IRequest<IEnumerable<OrganisationSearchResult>>
    {
        public OrganisationSearchRequest(string searchTerm)
        {
            SearchTerm = searchTerm;
        }

        public string SearchTerm { get; }
    }
}
