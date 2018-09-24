using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Questions
{
    public class CreateQuestionRequest : IRequest<Question>, IQuestionRequest
    {
        public string SequenceId { get; }
        public string SectionId { get; }
        public string PageId { get; }
        public Question Question { get; }

        public CreateQuestionRequest(string sequenceId, string sectionId, string pageId, Question question)
        {
            SequenceId = sequenceId;
            SectionId = sectionId;
            PageId = pageId;
            Question = question;
        }
    }
}