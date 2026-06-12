using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers
{
    public class MergeOrganisationsHandler : IRequestHandler<MergeOrganisationsRequest, MergeOrganisation>
    {
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IOrganisationMergingService _organisationMergingService;

        public MergeOrganisationsHandler(
            IOrganisationQueryRepository organisationQueryRepository
            , IOrganisationMergingService organisationMergingService
            )
        {
            _organisationQueryRepository = organisationQueryRepository;
            _organisationMergingService = organisationMergingService;
        }

        public async Task<MergeOrganisation> Handle(MergeOrganisationsRequest mergeOrganisationRequest, CancellationToken cancellationToken)
        {
            var primaryOrganisation = await _organisationQueryRepository.Get(mergeOrganisationRequest.PrimaryEndPointAssessorOrganisationId);
            var secondaryOrganisation = await _organisationQueryRepository.Get(mergeOrganisationRequest.SecondaryEndPointAssessorOrganisationId);

            var mergeOrganisation = await _organisationMergingService.MergeOrganisations(primaryOrganisation, secondaryOrganisation, mergeOrganisationRequest.SecondaryStandardsEffectiveTo, mergeOrganisationRequest.ActionedByUser);

            return mergeOrganisation;
        }
    }
}