using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Learner
{
    public class GetApprenticeLearnerHandler : IRequestHandler<GetApprenticeLearnerRequest, GetApprenticeLearnerResponse>
    {
        private readonly ILearnerRepository _learnerRepository;
        private readonly ILogger<GetApprenticeLearnerHandler> _logger;


        public GetApprenticeLearnerHandler(ILearnerRepository learnerRepository, ILogger<GetApprenticeLearnerHandler> logger)
        {
            _learnerRepository = learnerRepository;
            _logger = logger;
        }

        public async Task<GetApprenticeLearnerResponse> Handle(GetApprenticeLearnerRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Retrieving Learner with Apprentice Commitments Id = {request.ApprenticeshipId}");
            var learner = await _learnerRepository.Get(request.ApprenticeshipId);

            if (learner == null)
            {
                _logger.LogInformation($"Learner not found for Apprenticeship Id :{request.ApprenticeshipId}");
                return default;
            }
            
            var approvalsRecord = new GetApprenticeLearnerResponse()
            {
                ApprenticeshipId = learner.ApprenticeshipId.Value,
                Ukprn = learner.UkPrn,
                LearnStartDate= learner.LearnStartDate,
                PlannedEndDate = learner.PlannedEndDate,
                StandardCode = learner.StdCode,
                StandardUId = learner.StandardUId,
                StandardReference = learner.StandardReference,
                StandardName = learner.StandardName,
                CompletionStatus = learner.CompletionStatus,
                ApprovalsStopDate = learner.ApprovalsStopDate,
                ApprovalsPauseDate = learner.ApprovalsPauseDate,
                EstimatedEndDate = learner.EstimatedEndDate,
                Uln = learner.Uln,
                GivenNames = learner.GivenNames,
                FamilyName = learner.FamilyName,
                LearnActEndDate = learner.LearnActEndDate,
                IsTransfer = learner.IsTransfer,
                DateTransferIdentified = learner.DateTransferIdentified,
                ProviderName = learner.ProviderName
            };

            return approvalsRecord;
        }
    }
}