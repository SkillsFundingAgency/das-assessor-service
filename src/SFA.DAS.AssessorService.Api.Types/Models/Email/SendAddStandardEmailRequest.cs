using MediatR;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SendAddStandardEmailRequest : IRequest<Unit>
    {
        public string ContactId { get; set; }
        public string StandardReference { get; set; }
        public List<string> StandardVersions { get; set; }
    }
}
