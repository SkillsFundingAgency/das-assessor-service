using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Questions
{
    public class DeleteQuestionRequest : IRequest, IQuestionRequest
    {
        public string SequenceId { get; }
        public string SectionId { get; }
        public string PageId { get; }
        public string QuestionId { get; }

        public DeleteQuestionRequest(string sequenceId, string sectionId, string pageId, string questionId)
        {
            SequenceId = sequenceId;
            SectionId = sectionId;
            PageId = pageId;
            QuestionId = questionId;
        }
    }
}