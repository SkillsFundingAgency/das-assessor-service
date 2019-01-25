using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetBatchFromBatchNumberRequest : IRequest<BatchLogResponse>
    {
        public string BatchNumber { get; set; }
    }
}
