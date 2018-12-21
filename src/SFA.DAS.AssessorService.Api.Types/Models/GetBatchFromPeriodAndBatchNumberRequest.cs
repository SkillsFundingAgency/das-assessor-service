using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetBatchFromPeriodAndBatchNumberRequest : IRequest<BatchLogResponse>
    {
        public string Period { get; set; }
        public string BatchNumber { get; set; }
    }
}
