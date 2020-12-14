using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class CreateBatchLogRequest : IRequest<BatchLogResponse>
    {     
        public DateTime ScheduledDate { get; set; }
    }
}
