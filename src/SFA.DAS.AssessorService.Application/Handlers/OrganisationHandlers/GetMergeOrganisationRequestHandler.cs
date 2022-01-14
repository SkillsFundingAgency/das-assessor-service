using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers
{
    public class GetMergeOrganisationRequestHandler : IRequestHandler<GetMergeOrganisationRequest, GetMergeOrganisationResponse>
    {
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public GetMergeOrganisationRequestHandler(IOrganisationQueryRepository organisationQueryRepository)
        {
            _organisationQueryRepository = organisationQueryRepository;
        }

        public async Task<GetMergeOrganisationResponse> Handle(GetMergeOrganisationRequest request,
            CancellationToken cancellationToken)
        {
            var mergeOrganisation = await _organisationQueryRepository.GetMergeOrganisation(request.Id);
            if (null == mergeOrganisation) return null;
            return new GetMergeOrganisationResponse() 
            { 
                 Id = mergeOrganisation.Id,
                 PrimaryEndPointAssessorOrganisationId = mergeOrganisation.PrimaryEndPointAssessorOrganisationId,
                 SecondaryEndPointAssessorOrganisationId = mergeOrganisation.SecondaryEndPointAssessorOrganisationId,
                 SecondaryEPAOEffectiveTo = mergeOrganisation.SecondaryEPAOEffectiveTo,
                 CompletionDate = mergeOrganisation.CompletedAt
            };
        }
    }
}
