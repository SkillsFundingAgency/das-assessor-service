using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class AssessmentOrgsImportRequest: IRequest<AssessmentOrgsImportResponse>
    {
        public string Operation { get; set; }
    }
}
