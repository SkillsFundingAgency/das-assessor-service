using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class ResetApplicationToStage1Request : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }
}
