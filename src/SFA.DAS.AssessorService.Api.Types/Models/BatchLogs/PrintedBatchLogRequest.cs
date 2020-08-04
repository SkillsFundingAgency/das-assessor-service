using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class PrintedBatchLogRequest : IRequest<ValidationResponse>
    {
        public int BatchNumber { get; set; }
        public DateTime PrintedAt { get; set; }
    }
}
