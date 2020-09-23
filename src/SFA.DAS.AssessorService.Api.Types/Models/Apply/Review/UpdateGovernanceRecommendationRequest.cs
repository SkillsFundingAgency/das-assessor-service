using MediatR;
using SFA.DAS.AssessorService.ApplyTypes;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Review
{
    public class UpdateGovernanceRecommendationRequest : IRequest
    {
        public Guid Id { get; }
        public GovernanceRecommendation UpdatedRecomendation { get; }

        public UpdateGovernanceRecommendationRequest(Guid id, GovernanceRecommendation updatedRecommendation)
        {
            Id = id;
            UpdatedRecomendation = updatedRecommendation;
        }
    }
}
