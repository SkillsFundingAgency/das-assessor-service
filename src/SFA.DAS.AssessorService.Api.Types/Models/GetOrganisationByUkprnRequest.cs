using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOrganisationByUkprnRequest : IRequest<OrganisationResponse>
    {
        public long Ukprn { get; }

        public GetOrganisationByUkprnRequest(long ukprn)
        {
            Ukprn = ukprn;
        }
    }
}