using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Pages;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Questions;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config.Questions
{
    public class DeleteQuestionRequestHandler : QuestionHandlerBase, IRequestHandler<DeleteQuestionRequest>
    {
        public DeleteQuestionRequestHandler(IApplyRepository applyRepository) : base(applyRepository){}
        
        public async Task Handle(DeleteQuestionRequest request, CancellationToken cancellationToken)
        {
            var page = await GetPage(request);
            
            var questions = page.Questions;
            
            questions.RemoveAll(s => s.QuestionId == request.QuestionId);

            var newOrder = 0;
            questions.ForEach(s => s.Order = newOrder++);

            await StoreWorkflow();
        }
    }
}