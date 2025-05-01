using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Assessments
{
    public class UpdateAssessmentsSummaryHandler : IRequestHandler<UpdateAssessmentsSummaryRequest, Unit>
    {
        private readonly ILogger<UpdateAssessmentsSummaryHandler> _logger;
        private readonly ICertificateRepository _certificateRepository;

        public UpdateAssessmentsSummaryHandler(ILogger<UpdateAssessmentsSummaryHandler> logger, ICertificateRepository certificateRepository)
        {
            _logger = logger;
			_certificateRepository = certificateRepository;
        }

        public async Task<Unit> Handle(UpdateAssessmentsSummaryRequest request, CancellationToken cancellationToken)
		{
            await _certificateRepository.UpdateAssessmentsSummary();
			return Unit.Value;
		}
    }
}
