using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Questions
{
    public class GetQuestionsRequest : IRequest<List<Question>>, IQuestionRequest
    {
        public string SequenceId { get; }
        public string SectionId { get; }
        public string PageId { get; }

        public GetQuestionsRequest(string sequenceId, string sectionId, string pageId)
        {
            SequenceId = sequenceId;
            SectionId = sectionId;
            PageId = pageId;
        }
    }
}