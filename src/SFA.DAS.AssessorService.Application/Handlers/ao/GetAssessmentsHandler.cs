using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetAssessmentsHandler : IRequestHandler<GetAssessmentsRequest, GetAssessmentsResponse>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GetAssessmentsHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<GetAssessmentsResponse> Handle(GetAssessmentsRequest request, CancellationToken cancellationToken)
        {
            var result = await _certificateRepository.GetAssessments(request.Ukprn, request.StandardReference);
            return new GetAssessmentsResponse(result.EarliestAssessment, result.EndpointAssessmentCount);
        }
    }
}
