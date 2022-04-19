using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetApprenticeLearnerRequest : IRequest<GetApprenticeLearnerResponse>
    {
        public GetApprenticeLearnerRequest(long apprenticeshipId)
        {
            ApprenticeshipId = apprenticeshipId;
        }

        /// <summary>
        /// Apprenticeship Commitments Id
        /// </summary>
        public long ApprenticeshipId { get; }
    }
}