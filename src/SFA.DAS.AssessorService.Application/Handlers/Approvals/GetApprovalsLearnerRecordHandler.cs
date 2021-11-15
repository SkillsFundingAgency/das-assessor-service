using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Learner;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Approvals
{
    public class GetApprovalsLearnerRecordHandler : IRequestHandler<GetApprovalsLearnerRecordRequest, ApprovalsLearnerResult>
    {
        private readonly ILearnerRepository _learnerRepository;
        private readonly ILogger<GetApprovalsLearnerRecordHandler> _logger;

        public GetApprovalsLearnerRecordHandler(ILearnerRepository learnerRepository, ILogger<GetApprovalsLearnerRecordHandler> logger)
        {
            _learnerRepository = learnerRepository;
            _logger = logger;
        }

        public async Task<ApprovalsLearnerResult> Handle(GetApprovalsLearnerRecordRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting GetApprovalsLearnerRecordRequest for ULN:{request.Uln}, StandardCode:{request.StdCode}");
            var learner = await _learnerRepository.Get(request.Uln, request.StdCode);

            if (learner == null)
            {
                _logger.LogInformation($"Learner not found for ULN:{request.Uln}, StandardCode:{request.StdCode}");
                return default;
            }

            var approvalsRecord = new ApprovalsLearnerResult()
            {
                Uln = request.Uln,
                FamilyName = learner.FamilyName,
                GivenNames = learner.GivenNames,
                StandardCode = learner.StdCode,
                Version = learner.Version,
                VersionConfirmed = learner.VersionConfirmed,
                CourseOption = learner.CourseOption
            };

            return approvalsRecord;
        }

    }
}