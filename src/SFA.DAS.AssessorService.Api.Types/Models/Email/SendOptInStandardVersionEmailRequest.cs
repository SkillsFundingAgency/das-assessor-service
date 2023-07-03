using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SendOptInStandardVersionEmailRequest : IRequest
    {
        public Guid ContactId { get; set; }

        public string StandardReference { get; set; }

        public string Version { get; set; }
    }
}
