using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetLastBatchLogRequest : IRequest<BatchLogResponse>
    {
    }
}