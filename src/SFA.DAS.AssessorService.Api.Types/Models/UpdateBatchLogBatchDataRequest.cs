using System;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateBatchLogBatchDataRequest : IRequest<ValidationResponse>
    {
        public Guid Id { get; set; }
        public string BatchData { get; set; }
    }
}
