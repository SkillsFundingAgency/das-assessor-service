using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sections
{
    public class GetSectionsRequest : IRequest<List<SectionSummary>>
    {
        public string SequenceId { get; }

        public GetSectionsRequest(string sequenceId)
        {
            SequenceId = sequenceId;
        }
    }
}