using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Questions
{
    public class GetQuestionRequest : IRequest<Question>, IQuestionRequest
    {
        public string SequenceId { get; }
        public string SectionId { get; set; }
        public string PageId { get; }
        public string QuestionId { get; }

        public GetQuestionRequest(string sequenceId, string sectionId, string pageId, string questionId)
        {
            SequenceId = sequenceId;
            SectionId = sectionId;
            PageId = pageId;
            QuestionId = questionId;
        }
    }
}