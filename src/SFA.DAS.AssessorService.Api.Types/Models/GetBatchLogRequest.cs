using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetBatchLogRequest : IRequest<BatchLogResponse>
    {
        public int BatchNumber { get; set; }
    }
}
