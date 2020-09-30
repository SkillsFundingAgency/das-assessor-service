using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetForBatchNumberBatchLogRequest : IRequest<BatchLogResponse>
    {
        public int BatchNumber { get; set; }
    }
}
