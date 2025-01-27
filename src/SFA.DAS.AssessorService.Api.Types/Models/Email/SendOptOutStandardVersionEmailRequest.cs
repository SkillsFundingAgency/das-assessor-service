using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SendOptOutStandardVersionEmailRequest : IRequest<Unit>
    {
        public Guid ContactId { get; set; }

        public string StandardReference { get; set; }

        public string Version { get; set; }
    }
}
