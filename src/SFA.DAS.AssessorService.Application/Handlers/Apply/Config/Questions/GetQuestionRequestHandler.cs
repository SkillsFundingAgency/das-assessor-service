using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Questions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config.Questions
{
    public class GetQuestionRequestHandler : QuestionHandlerBase, IRequestHandler<GetQuestionRequest, Question>
    {
        public GetQuestionRequestHandler(IApplyRepository applyRepository) : base(applyRepository){}
        
        public async Task<Question> Handle(GetQuestionRequest request, CancellationToken cancellationToken)
        {
            var page = await GetPage(request);

            return page.Questions.FirstOrDefault(q => q.QuestionId == request.QuestionId);
        }
    }
}