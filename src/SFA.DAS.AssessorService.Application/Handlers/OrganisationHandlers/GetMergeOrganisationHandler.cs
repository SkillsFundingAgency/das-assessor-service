using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers
{
    public class GetMergeOrganisationHandler : IRequestHandler<GetMergeOrganisationRequest, MergeLogEntry>
    {
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public GetMergeOrganisationHandler(IOrganisationQueryRepository organisationQueryRepository)
        {
            _organisationQueryRepository = organisationQueryRepository;
        }

        public async Task<MergeLogEntry> Handle(GetMergeOrganisationRequest request,
            CancellationToken cancellationToken)
        {
            var mergeOrganisation = await _organisationQueryRepository.GetOrganisationMergeLogById(request.Id);
            return mergeOrganisation;
        }
    }
}
