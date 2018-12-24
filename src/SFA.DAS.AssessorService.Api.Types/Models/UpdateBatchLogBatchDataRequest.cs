using System;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateBatchLogBatchDataRequest : IRequest<ValidationResponse>
    {
        public Guid Id { get; set; }
        public BatchData BatchData { get; set; }
    }
}
