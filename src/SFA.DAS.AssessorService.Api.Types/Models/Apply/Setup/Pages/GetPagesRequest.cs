using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Pages
{
    public class GetPagesRequest : IRequest<List<Page>>
    {
        public string SequenceId { get; }
        public string SectionId { get; }

        public GetPagesRequest(string sequenceId, string sectionId)
        {
            SequenceId = sequenceId;
            SectionId = sectionId;
        }
    }
}