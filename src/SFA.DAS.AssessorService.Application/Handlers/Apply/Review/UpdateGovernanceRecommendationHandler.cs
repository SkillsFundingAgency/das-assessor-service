using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class UpdateGovernanceRecommendationHandler : IRequestHandler<UpdateGovernanceRecommendationRequest>
    {
        private readonly IApplyRepository _applyRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IOrganisationRepository _organisationRepository;

        public UpdateGovernanceRecommendationHandler(IApplyRepository applyRepository, IOrganisationQueryRepository organisationQueryRepository, IOrganisationRepository organisationRepository)
        {
            _applyRepository = applyRepository;
            _organisationQueryRepository = organisationQueryRepository;
            _organisationRepository = organisationRepository;
        }

        public async Task<Unit> Handle(UpdateGovernanceRecommendationRequest request, CancellationToken cancellationToken)
        {           
            await _applyRepository.UpdateGovernanceRecommendation(request.Id, request.UpdatedRecomendation);
            return Unit.Value;
        }
    }
}
