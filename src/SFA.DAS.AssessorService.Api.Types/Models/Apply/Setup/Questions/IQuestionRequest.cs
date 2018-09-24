namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Questions
{
    public interface IQuestionRequest
    {
        string SequenceId { get; }
        string SectionId { get; }
        string PageId { get; }
    }
}